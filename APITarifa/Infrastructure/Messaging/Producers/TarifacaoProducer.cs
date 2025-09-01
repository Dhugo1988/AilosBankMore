using APITarifa.Infrastructure.Messaging.Messages;
using KafkaFlow.Producers;

namespace APITarifa.Infrastructure.Messaging.Producers
{
    public class TarifacaoProducer : ITarifacaoProducer
    {
        private readonly IProducerAccessor _producerAccessor;
        private readonly ILogger<TarifacaoProducer> _logger;

        public TarifacaoProducer(IProducerAccessor producerAccessor, ILogger<TarifacaoProducer> logger)
        {
            _producerAccessor = producerAccessor;
            _logger = logger;
        }

        public async Task SendAsync(TarifacaoRealizadaMessage message)
        {
            try
            {
                _logger.LogInformation("Enviando mensagem de tarifação realizada para conta {IdContaCorrente}", 
                    message.IdContaCorrente);

                var producer = _producerAccessor.GetProducer("tarifacao-producer");
                await producer.ProduceAsync(message.IdRequisicao, message);

                _logger.LogInformation("Mensagem de tarifação enviada com sucesso para conta {IdContaCorrente}", 
                    message.IdContaCorrente);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar mensagem de tarifação para conta {IdContaCorrente}", 
                    message.IdContaCorrente);
                throw;
            }
        }
    }
}
