using System.Text.Json.Serialization;

namespace APITarifa.Infrastructure.Messaging.Messages
{
    public class TarifacaoRealizadaMessage
    {
        [JsonPropertyName("idrequisicao")]
        public string IdRequisicao { get; set; } = string.Empty;
        [JsonPropertyName("idcontacorrente")]
        public string IdContaCorrente { get; set; } = string.Empty;
        [JsonPropertyName("valortarifado")]
        public decimal ValorTarifado { get; set; }
    }
}
