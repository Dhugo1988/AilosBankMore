using APITransferencia.Infrastructure.Data;
using APITransferencia.Infrastructure.Messaging.Producers;
using KafkaFlow;
using KafkaFlow.Serializer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
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

builder.Services.AddDbContext<TransferenciaDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("TransferenciaConnection") ?? "Data Source=Transferencia.db"));

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

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

builder.Services.AddHttpClient("ContaCorrente", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ContaCorrente:BaseUrl"] ?? "https://localhost:5001/");
});

builder.Services.AddScoped<ITransferenciaProducer, TransferenciaProducer>();

builder.Services.AddScoped<APITransferencia.Application.Services.IMappingService, APITransferencia.Application.Services.MappingService>();

builder.Services.AddHealthChecks();

var kafkaBootstrapServers = builder.Configuration["Kafka:BootstrapServers"] ?? "kafka:9092";
var transferenciasTopic = builder.Configuration["Kafka:Topics:TransferenciasRealizadas"] ?? "transferencias-realizadas";

builder.Services.AddKafka(kafka => kafka
    .AddCluster(cluster => cluster
        .WithBrokers(new[] { kafkaBootstrapServers })
        .AddProducer("transferencia-producer", producer => producer
            .DefaultTopic(transferenciasTopic)
            .AddMiddlewares(middlewares => middlewares
                .AddSerializer<JsonCoreSerializer>()
            )
        )
    )
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TransferenciaDbContext>();
    db.Database.EnsureCreated();
}

var logger = app.Services.GetRequiredService<ILogger<Program>>();
var maxRetries = 5;
var retryDelay = TimeSpan.FromSeconds(10);
var isKafkaStarted = false;

for (int i = 0; i < maxRetries; i++)
{
    try
    {   
        var kafkaBus = app.Services.CreateKafkaBus();
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        await kafkaBus.StartAsync(cts.Token);
        
        isKafkaStarted = true;
        break;
    }
    catch (OperationCanceledException)
    {
        logger.LogWarning("Timeout ao iniciar Kafka (tentativa {Attempt}/{MaxRetries})", i + 1, maxRetries);
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Falha ao iniciar Kafka (tentativa {Attempt}/{MaxRetries}): {Message}", i + 1, maxRetries, ex.Message);
    }
    
    if (i < maxRetries - 1)
    {
        await Task.Delay(retryDelay);
    }
}

if (!isKafkaStarted)
{
    logger.LogError("Não foi possível iniciar Kafka após {MaxRetries} tentativas. Consumer não estará disponível.", maxRetries);
}

app.Run();
