
using System.Text.Json.Serialization;

namespace APITransferencia.Application.DTOs
{
    public class ConsultarContaCorrenteIdResponse : BaseResponseDto
    {
        [JsonPropertyName("contaCorrenteId")]
        public string ContaCorrenteId { get; set; } = string.Empty;
    }
}
