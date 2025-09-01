using System.Text.Json;
using Confluent.Kafka;
using APITarifa.Domain.Repositories;
using APITarifa.Domain.Entities;
using APITarifa.Infrastructure.Messaging.Messages;
using APITarifa.Infrastructure.Messaging.Producers;

namespace APITarifa.Infrastructure.Messaging.Hosted
{
    public class TransferenciasConsumerHostedService : BackgroundService
    {
        private readonly ILogger<TransferenciasConsumerHostedService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public TransferenciasConsumerHostedService(
            ILogger<TransferenciasConsumerHostedService> logger,
            IConfiguration configuration,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _configuration = configuration;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var bootstrap = _configuration["Kafka:BootstrapServers"] ?? "kafka:9092";
            var topic = _configuration["Kafka:Topics:TransferenciasRealizadas"] ?? "transferencias-realizadas";
            var groupId = (_configuration["Kafka:GroupId"] ?? "tarifa-service") + "-hosted";

            var config = new ConsumerConfig
            {
                BootstrapServers = bootstrap,
                GroupId = groupId,
                EnableAutoCommit = true,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(topic);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var cr = consumer.Consume(TimeSpan.FromSeconds(1));
                    if (cr is null)
                    {
                        continue;
                    }

                    var raw = cr.Message.Value;

                    var msg = JsonSerializer.Deserialize<TransferenciaRealizadaMessage>(raw, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (msg is null)
                    {
                        _logger.LogWarning("[Hosted] Mensagem inválida para deserialização");
                        continue;
                    }

                    using var scope = _serviceProvider.CreateScope();
                    var tarifaRepository = scope.ServiceProvider.GetRequiredService<ITarifaRepository>();
                    var idempotenciaRepository = scope.ServiceProvider.GetRequiredService<IIdempotenciaRepository>();
                    var tarifacaoProducer = scope.ServiceProvider.GetRequiredService<ITarifacaoProducer>();

                    if (await idempotenciaRepository.ExistsAsync(msg.IdRequisicao))
                    {
                        _logger.LogInformation("[Hosted] Ignorando mensagem já processada: {Id}", msg.IdRequisicao);
                        continue;
                    }

                    var valorTarifa = _configuration.GetValue<decimal>("Tarifa:ValorTransferencia");
                    var tarifa = new Tarifa
                    {
                        IdContaCorrente = msg.IdContaCorrente,
                        DataMovimento = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
                        Valor = valorTarifa
                    };
                    await tarifaRepository.AddAsync(tarifa);

                    var idem = new Idempotencia
                    {
                        ChaveIdempotencia = msg.IdRequisicao,
                        Requisicao = raw,
                        Resultado = JsonSerializer.Serialize(tarifa)
                    };
                    await idempotenciaRepository.AddAsync(idem);

                    await tarifacaoProducer.SendAsync(new TarifacaoRealizadaMessage
                    {
                        IdRequisicao = Guid.NewGuid().ToString(),
                        IdContaCorrente = msg.IdContaCorrente,
                        ValorTarifado = valorTarifa
                    });

                    _logger.LogInformation("[Hosted] Tarifa aplicada: Conta={Conta}, Valor={Valor}", msg.IdContaCorrente, valorTarifa);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[Hosted] Erro no consumo");
                    await Task.Delay(1000, stoppingToken);
                }
            }

            try
            {
                consumer.Close();
            }
            catch { }

            _logger.LogInformation("Hosted consumer finalizado");
        }
    }
}



