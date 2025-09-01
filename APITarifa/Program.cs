using APITarifa.Domain.Repositories;
using APITarifa.Infrastructure.Data;
using APITarifa.Infrastructure.Messaging.Producers;
using APITarifa.Infrastructure.Repositories;
using KafkaFlow;
using KafkaFlow.Serializer;
using Confluent.Kafka;
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

// Configuração do MediatR para CQRS
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
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

builder.Services.AddHealthChecks();

builder.Services.AddSingleton<TarifaDbContext>(provider =>
{
    var options = new DbContextOptionsBuilder<TarifaDbContext>()
        .UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
        .Options;
    return new TarifaDbContext(options);
});

builder.Services.AddSingleton<ITarifaRepository, TarifaRepository>();
builder.Services.AddSingleton<IIdempotenciaRepository, IdempotenciaRepository>();

builder.Services.AddSingleton<ITarifacaoProducer, TarifacaoProducer>();

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key não configurada");
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
var kafkaGroupId = builder.Configuration["Kafka:GroupId"] ?? "tarifa-service";
var transferenciasTopic = builder.Configuration["Kafka:Topics:TransferenciasRealizadas"] ?? "transferencias-realizadas";
var tarifacoesTopic = builder.Configuration["Kafka:Topics:TarifacoesRealizadas"] ?? "tarifacoes-realizadas";

builder.Services.AddHostedService<APITarifa.Infrastructure.Messaging.Hosted.TransferenciasConsumerHostedService>();

builder.Services
    .AddKafka(kafka => kafka
    .AddCluster(cluster => cluster
        .WithBrokers(new[] { kafkaBootstrapServers })
        .WithName("kafka-cluster")
        .AddProducer("tarifacao-producer", producer => producer
            .DefaultTopic(tarifacoesTopic)
            .AddMiddlewares(middlewares => middlewares
                .AddSerializer<JsonCoreSerializer>()
            )
        )
    )
);

var app = builder.Build();

// Criar banco de dados se não existir
var context = app.Services.GetRequiredService<TarifaDbContext>();
context.Database.EnsureCreated();

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
try
{
    var kafkaBus = app.Services.CreateKafkaBus();
    await kafkaBus.StartAsync();
}
catch (Exception ex)
{
    logger.LogError(ex, "Erro ao iniciar Kafka bus: {Message}", ex.Message);
    if (ex.InnerException != null)
    {
        logger.LogError("Inner exception: {InnerMessage}", ex.InnerException.Message);
    }
}

app.Run();

