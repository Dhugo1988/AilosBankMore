using APIContaCorrente.Domain.Repositories;
using APIContaCorrente.Domain.Services;
using APIContaCorrente.Infrastructure.Data;
using APIContaCorrente.Infrastructure.Repositories;
using KafkaFlow;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
//builder.WebHost.UseUrls("http://0.0.0.0:8080");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHealthChecks();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddDbContext<ContaCorrenteDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddScoped<IContaCorrenteRepository, ContaCorrenteRepository>();
builder.Services.AddScoped<IMovimentoRepository, MovimentoRepository>();
builder.Services.AddScoped<IIdempotenciaRepository, IdempotenciaRepository>();

builder.Services.AddScoped<ICpfValidationService, APIContaCorrente.Infrastructure.Services.CpfValidationService>();
builder.Services.AddScoped<IContaCorrenteService, APIContaCorrente.Infrastructure.Services.ContaCorrenteService>();
builder.Services.AddScoped<IPasswordHasherService, APIContaCorrente.Infrastructure.Security.PasswordHasherService>();

builder.Services.AddScoped<APIContaCorrente.Application.Services.IMappingService, APIContaCorrente.Application.Services.MappingService>();
builder.Services.AddScoped<APIContaCorrente.Application.Services.IValidationService, APIContaCorrente.Application.Services.ValidationService>();

builder.Services.AddSingleton<APIContaCorrente.Infrastructure.Security.JwtTokenService>(provider =>
            {
                var configuration = provider.GetRequiredService<IConfiguration>();
                var key = configuration["Jwt:Key"];
                var issuer = configuration["Jwt:Issuer"];
                var audience = configuration["Jwt:Audience"];
                return new APIContaCorrente.Infrastructure.Security.JwtTokenService(key, issuer, audience);
            });

var jwtKey = builder.Configuration["Jwt:Key"];
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ValidIssuer = "BankMore",
        ValidAudience = "BankMore"
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            context.NoResult();
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync("{\"error\": \"Token inválido\"}");
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

var kafkaBootstrapServers = builder.Configuration["Kafka:BootstrapServers"] ?? "kafka:9092";
var kafkaGroupId = builder.Configuration["Kafka:GroupId"] ?? "contacorrente-service";
var tarifacoesTopic = builder.Configuration["Kafka:Topics:TarifacoesRealizadas"] ?? "tarifacoes-realizadas";

builder.Services.AddHostedService<APIContaCorrente.Infrastructure.Messaging.Hosted.TarifacoesConsumerHostedService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ContaCorrenteDbContext>();
    context.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapHealthChecks("/health").AllowAnonymous();

var logger = app.Services.GetRequiredService<ILogger<Program>>();

app.Run();
