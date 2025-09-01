using APITarifa.Application.DTOs;

namespace APITarifa.Application.Commands.ConsultarTarifasPorConta
{
    public class ConsultarTarifasPorContaResponse
    {
        public bool Success { get; set; }
        public string? ErrorType { get; set; }
        public string? Message { get; set; }
        public TarifaResponseDto? Data { get; set; }

        public static ConsultarTarifasPorContaResponse CreateSuccess(TarifaResponseDto data)
        {
            return new ConsultarTarifasPorContaResponse
            {
                Success = true,
                Data = data
            };
        }

        public static ConsultarTarifasPorContaResponse CreateFailure(string errorType, string message)
        {
            return new ConsultarTarifasPorContaResponse
            {
                Success = false,
                ErrorType = errorType,
                Message = message
            };
        }
    }
}
