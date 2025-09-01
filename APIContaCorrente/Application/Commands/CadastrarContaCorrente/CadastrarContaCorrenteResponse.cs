using APIContaCorrente.Application.Common.Model;

namespace APIContaCorrente.Application.Commands.CadastrarContaCorrente
{
    public class CadastrarContaCorrenteResponse : BaseResponse
    {
        public int? NumeroConta { get; set; }

        public CadastrarContaCorrenteResponse(bool success, string? message = null, string? errorType = null)
            : base(success, message, errorType)
        {
        }

        public CadastrarContaCorrenteResponse(bool success, int? numeroConta = null) : base(success)
        {
            NumeroConta = numeroConta;
        }
    }
}
