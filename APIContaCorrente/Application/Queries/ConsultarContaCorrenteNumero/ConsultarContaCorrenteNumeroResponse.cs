using APIContaCorrente.Application.Common.Model;

namespace APIContaCorrente.Application.Queries.ConsultarContaCorrente
{
    public class ConsultarContaCorrenteNumeroResponse : BaseResponse
    {
        public int? NumeroConta { get; set; }

        public ConsultarContaCorrenteNumeroResponse(bool success, string? message = null, string? errorType = null)
            : base(success, message, errorType)
        {
        }

        public ConsultarContaCorrenteNumeroResponse(bool success, int? numeroConta) : base(success)
        {
            NumeroConta = numeroConta;
        }
    }
}