using APIContaCorrente.Application.Commands.Movimentar;
using APIContaCorrente.Application.Common.Model;
using APIContaCorrente.Domain.Entities;

namespace APIContaCorrente.Application.Common.Validators
{
    public class AccountValidationResult
    {
        public bool IsValid { get; private set; }
        public ContaCorrente ContaCorrente { get; private set; }
        public BaseResponse ErrorResponse { get; private set; }

        private AccountValidationResult(bool isValid, ContaCorrente contaCorrente = null, BaseResponse errorResponse = null)
        {
            IsValid = isValid;
            ContaCorrente = contaCorrente;
            ErrorResponse = errorResponse;
        }

        public static AccountValidationResult Valid(ContaCorrente contaCorrente) => new(true, contaCorrente);
        public static AccountValidationResult Invalid(BaseResponse errorResponse) => new(false, null, errorResponse);
    }
}
