
using System.Text.Json.Serialization;

namespace APITransferencia.Application.DTOs
{
    public class ConsultarContaCorrenteNumeroResponse : BaseResponseDto
    {
        [JsonPropertyName("numeroConta")]
        public int? NumeroConta { get; set; }
    }
}
