//using APITarifa.Domain.Entities;
//using APITarifa.Domain.Repositories;
//using APITarifa.Infrastructure.Messaging.Producers;
//using APITarifa.Infrastructure.Messaging.Messages;
//using KafkaFlow;
//using System.Text.Json;
//using System.Text;

//namespace APITarifa.Infrastructure.Messaging.Handlers
//{
//    public class TransferenciaRealizadaHandler : IMessageHandler<byte[]>
//    {
//        private readonly ITarifaRepository _tarifaRepository;
//        private readonly IIdempotenciaRepository _idempotenciaRepository;
//        private readonly ITarifacaoProducer _tarifacaoProducer;
//        private readonly IConfiguration _configuration;
//        private readonly ILogger<TransferenciaRealizadaHandler> _logger;

//        // Constantes para configuração
//        private const string TARIFA_CONFIG_KEY = "Tarifa:ValorTransferencia";
//        private const decimal TARIFA_DEFAULT_VALUE = 2.00m;
//        private const string TARIFA_PREFIX = "tarifa-";
        
//        // Constantes para formato de data
//        private const string DATE_FORMAT = "dd/MM/yyyy HH:mm:ss";
        
//        // Constantes para mensagens de log
//        private const string LOG_PROCESSING_STARTED = "Processando transferência realizada para conta {IdContaCorrente}";
//        private const string LOG_ALREADY_PROCESSED = "Mensagem já processada: {IdRequisicao}";
//        private const string LOG_SUCCESS = "Tarifa aplicada com sucesso. Conta: {IdContaCorrente}, Valor: {Valor}";
//        private const string LOG_ERROR = "Erro ao processar transferência realizada para conta {IdContaCorrente}";

//        public TransferenciaRealizadaHandler(
//            ITarifaRepository tarifaRepository,
//            IIdempotenciaRepository idempotenciaRepository,
//            ITarifacaoProducer tarifacaoProducer,
//            IConfiguration configuration,
//            ILogger<TransferenciaRealizadaHandler> logger)
//        {
//            _tarifaRepository = tarifaRepository;
//            _idempotenciaRepository = idempotenciaRepository;
//            _tarifacaoProducer = tarifacaoProducer;
//            _configuration = configuration;
//            _logger = logger;
//            _logger.LogInformation("🔧 TransferenciaRealizadaHandler registrado");
//        }

//        public async Task Handle(IMessageContext context, byte[] messageBytes)
//        {
//            _logger.LogInformation("🎯 === HANDLER INICIADO (raw) ===");
//            var rawJson = Encoding.UTF8.GetString(messageBytes);
//            _logger.LogInformation("📦 Mensagem recebida (raw): {MessageJson}", rawJson);

//            var message = JsonSerializer.Deserialize<TransferenciaRealizadaMessage>(rawJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
//                          ?? new TransferenciaRealizadaMessage();
//            _logger.LogInformation("📋 IdRequisicao: {IdRequisicao}", message.IdRequisicao);
//            _logger.LogInformation("🏦 IdContaCorrente: {IdContaCorrente}", message.IdContaCorrente);
//            _logger.LogInformation("📅 DataMovimento: {DataMovimento}", message.DataMovimento);
            
//            try
//            {
//                _logger.LogInformation("🔑 Gerando chave de idempotência...");
//                var chaveIdempotencia = GenerateIdempotencyKey(message.IdRequisicao);
//                _logger.LogInformation("🔑 Chave gerada: {ChaveIdempotencia}", chaveIdempotencia);
                
//                _logger.LogInformation("🔍 Verificando se mensagem já foi processada...");
//                if (await _idempotenciaRepository.ExistsAsync(chaveIdempotencia))
//                {
//                    _logger.LogWarning(LOG_ALREADY_PROCESSED, message.IdRequisicao);
//                    return;
//                }
//                _logger.LogInformation("✅ Mensagem não foi processada anteriormente, prosseguindo...");

//                // Processar tarifa
//                _logger.LogInformation("💰 Obtendo valor da tarifa...");
//                var valorTarifa = GetTarifaValue();
//                _logger.LogInformation("💰 Valor da tarifa: {ValorTarifa}", valorTarifa);
                
//                _logger.LogInformation("📊 Criando registro de tarifa...");
//                var tarifa = CreateTarifa(message, valorTarifa);
//                _logger.LogInformation("📊 Tarifa criada: {TarifaJson}", JsonSerializer.Serialize(tarifa));
                
//                _logger.LogInformation("💾 Salvando tarifa no banco...");
//                await _tarifaRepository.AddAsync(tarifa);
//                _logger.LogInformation("✅ Tarifa salva com sucesso!");

//                _logger.LogInformation("🔐 Criando registro de idempotência...");
//                var idempotencia = CreateIdempotencia(chaveIdempotencia, message, tarifa);
//                _logger.LogInformation("💾 Salvando idempotência no banco...");
//                await _idempotenciaRepository.AddAsync(idempotencia);
//                _logger.LogInformation("✅ Idempotência salva com sucesso!");

//                // Notificar conclusão da tarifação
//                _logger.LogInformation("📨 Enviando notificação de tarifação...");
//                var tarifacaoMessage = CreateTarifacaoMessage(message.IdContaCorrente, valorTarifa);
//                _logger.LogInformation("📨 Mensagem de tarifação: {TarifacaoMessageJson}", JsonSerializer.Serialize(tarifacaoMessage));
//                await _tarifacaoProducer.SendAsync(tarifacaoMessage);
//                _logger.LogInformation("✅ Notificação enviada com sucesso!");

//                _logger.LogInformation(LOG_SUCCESS, message.IdContaCorrente, valorTarifa);
//                _logger.LogInformation("🎉 === HANDLER CONCLUÍDO COM SUCESSO ===");
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, LOG_ERROR, message.IdContaCorrente);
//                _logger.LogError("❌ === ERRO NO HANDLER KAFKA ===");
//                _logger.LogError("🔍 Stack trace: {StackTrace}", ex.StackTrace);
                
//                // Em produção, considere enviar para dead letter queue
//                // await _deadLetterProducer.SendToDeadLetterQueue(message, ex);
                
//                throw; // Re-throw para que o Kafka possa fazer retry se configurado
//            }
//        }

//        private static string GenerateIdempotencyKey(string idRequisicao)
//        {
//            return TARIFA_PREFIX + idRequisicao;
//        }

//        private decimal GetTarifaValue()
//        {
//            return _configuration.GetValue<decimal>(TARIFA_CONFIG_KEY, TARIFA_DEFAULT_VALUE);
//        }

//        private static Tarifa CreateTarifa(TransferenciaRealizadaMessage message, decimal valorTarifa)
//        {
//            return new Tarifa
//            {
//                IdContaCorrente = message.IdContaCorrente,
//                DataMovimento = DateTime.Now.ToString(DATE_FORMAT),
//                Valor = valorTarifa
//            };
//        }

//        private static Idempotencia CreateIdempotencia(string chaveIdempotencia, TransferenciaRealizadaMessage message, Tarifa tarifa)
//        {
//            return new Idempotencia
//            {
//                ChaveIdempotencia = chaveIdempotencia,
//                Requisicao = JsonSerializer.Serialize(message),
//                Resultado = JsonSerializer.Serialize(tarifa)
//            };
//        }

//        private static TarifacaoRealizadaMessage CreateTarifacaoMessage(string idContaCorrente, decimal valorTarifado)
//        {
//            return new TarifacaoRealizadaMessage
//            {
//                IdRequisicao = Guid.NewGuid().ToString(),
//                IdContaCorrente = idContaCorrente,
//                ValorTarifado = valorTarifado
//            };
//        }
//    }
//}