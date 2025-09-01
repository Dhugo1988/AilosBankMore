using APIContaCorrente.Application.Common.Model;

namespace APIContaCorrente.Application.Queries.ConsultarContaCorrente
{
    public class ConsultarContaCorrenteResponse : BaseResponse
    {
        public string ContaCorrenteId { get; set; } = string.Empty;

        public ConsultarContaCorrenteResponse(bool success, string? message = null, string? errorType = null)
            : base(success, message, errorType)
        {
        }

        public ConsultarContaCorrenteResponse(bool success, string contaCorrenteId = null) : base(success)
        {
            ContaCorrenteId = contaCorrenteId;
        }
    }
}