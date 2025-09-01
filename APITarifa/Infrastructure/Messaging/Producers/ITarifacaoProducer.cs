using APITarifa.Infrastructure.Messaging.Messages;

namespace APITarifa.Infrastructure.Messaging.Producers
{
    public interface ITarifacaoProducer
    {
        Task SendAsync(TarifacaoRealizadaMessage message);
    }
}
