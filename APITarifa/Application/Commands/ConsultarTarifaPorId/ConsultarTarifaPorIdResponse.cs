using APITarifa.Domain.Entities;

namespace APITarifa.Application.Commands.ConsultarTarifaPorId
{
    public class ConsultarTarifaPorIdResponse
    {
        public bool Success { get; set; }
        public string? ErrorType { get; set; }
        public string? Message { get; set; }
        public Tarifa? Data { get; set; }

        public static ConsultarTarifaPorIdResponse CreateSuccess(Tarifa data)
        {
            return new ConsultarTarifaPorIdResponse
            {
                Success = true,
                Data = data
            };
        }

        public static ConsultarTarifaPorIdResponse CreateFailure(string errorType, string message)
        {
            return new ConsultarTarifaPorIdResponse
            {
                Success = false,
                ErrorType = errorType,
                Message = message
            };
        }
    }
}
