using System.Text.Json.Serialization;
namespace APITransferencia.Application.DTOs
{
    public class BaseResponseDto
    {
        [JsonPropertyName("success")]  
        public bool Success { get; set; }

        [JsonPropertyName("errorType")]
        public string? ErrorType { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}
