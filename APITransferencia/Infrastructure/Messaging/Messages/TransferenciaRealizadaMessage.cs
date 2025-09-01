using System.Text.Json.Serialization;

namespace APITransferencia.Infrastructure.Messaging.Messages
{
    public class TransferenciaRealizadaMessage
    {
        [JsonPropertyName("IdRequisicao")]
        public string IdRequisicao { get; set; } = string.Empty;
        [JsonPropertyName("IdContaCorrente")]
        public string IdContaCorrente { get; set; } = string.Empty;
        [JsonPropertyName("DataMovimento")]
        public string DataMovimento { get; set; } = string.Empty;
    }
}
