using APIContaCorrente.Application.Commands.Movimentar;
using APIContaCorrente.Application.Common.Constants;
using APIContaCorrente.Application.Queries.ConsultarContaCorrente;
using APIContaCorrente.Infrastructure.Messaging.Messages;
using Confluent.Kafka;
using MediatR;
using System.Text.Json;

namespace APIContaCorrente.Infrastructure.Messaging.Hosted
{
    public class TarifacoesConsumerHostedService : BackgroundService
    {
        private readonly ILogger<TarifacoesConsumerHostedService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public TarifacoesConsumerHostedService(
            ILogger<TarifacoesConsumerHostedService> logger,
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
            var topic = _configuration["Kafka:Topics:TarifacoesRealizadas"] ?? "tarifacoes-realizadas";
            var groupId = (_configuration["Kafka:GroupId"] ?? "contacorrente-service") + "-hosted";

            var config = new ConsumerConfig
            {
                BootstrapServers = bootstrap,
                GroupId = groupId,
                EnableAutoCommit = true,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(topic);
            _logger.LogInformation("[Hosted-Conta] Consumidor iniciado Topic={Topic} Group={Group}", topic, groupId);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var cr = consumer.Consume(TimeSpan.FromSeconds(1));
                    if (cr is null) { continue; }

                    var raw = cr.Message.Value;
                    _logger.LogInformation("[Hosted-Conta] Recebido: {Raw}", raw);

                    using var scope = _serviceProvider.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<TarifacoesConsumerHostedService>>();

                    var msg = JsonSerializer.Deserialize<TarifacaoRealizadaMessage>(raw, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    if (msg is null)
                    {
                        logger.LogWarning("[Hosted-Conta] Mensagem inválida");
                        continue;
                    }

                    var query = new ConsultarContaCorrenteNumeroQuery { ContaId = msg.IdContaCorrente };
                    var responseContaNumero = await mediator.Send(query);

                    var command = new MovimentarCommand
                    {
                        IdRequisicao = msg.IdRequisicao,
                        ContaCorrenteId = msg.IdContaCorrente,
                        Valor = msg.ValorTarifado,
                        TipoMovimento = ValidationConstants.TIPO_DEBITO,
                        NumeroConta = responseContaNumero.NumeroConta
                    };

                    var result = await mediator.Send(command, stoppingToken);
                    if (!result.Success)
                    {
                        logger.LogWarning("[Hosted-Conta] Falha ao debitar tarifa: {Msg}", result.Message);
                    }
                    else
                    {
                        logger.LogInformation("[Hosted-Conta] Tarifa debitada Conta={Conta} Valor={Valor}", msg.IdContaCorrente, msg.ValorTarifado);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[Hosted-Conta] Erro no consumo");
                    await Task.Delay(1000, stoppingToken);
                }
            }

            try { consumer.Close(); } catch { }
            _logger.LogInformation("[Hosted-Conta] Finalizado");
        }
    }
}


