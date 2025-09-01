using APITransferencia.Infrastructure.Messaging.Messages;

namespace APITransferencia.Infrastructure.Messaging.Producers
{
    public interface ITransferenciaProducer
    {
        Task TransferAsync(TransferenciaRealizadaMessage message);
    }
}
