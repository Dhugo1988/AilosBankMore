using APITransferencia.Infrastructure.Messaging.Messages;
using KafkaFlow.Producers;
using System.Text.Json;

namespace APITransferencia.Infrastructure.Messaging.Producers
{
    public class TransferenciaProducer : ITransferenciaProducer
    {
        private readonly IProducerAccessor _producerAccessor;
        private readonly ILogger<TransferenciaProducer> _logger;

        public TransferenciaProducer(IProducerAccessor producerAccessor, ILogger<TransferenciaProducer> logger)
        {
            _producerAccessor = producerAccessor;
            _logger = logger;
        }

        public async Task TransferAsync(TransferenciaRealizadaMessage message)
        {
            try
            {
                var producer = _producerAccessor.GetProducer("transferencia-producer");
                await producer.ProduceAsync("transferencias-realizadas", message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar mensagem de transferência para conta {IdContaCorrente}", 
                    message.IdContaCorrente);
                throw;
            }
        }
    }
}
