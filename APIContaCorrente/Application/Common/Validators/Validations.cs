using APIContaCorrente.Application.Commands.Movimentar;
using APIContaCorrente.Application.Common.Constants;
using APIContaCorrente.Domain.Repositories;

namespace APIContaCorrente.Application.Common.Validators
{
    public class Validations
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;

        public Validations(IContaCorrenteRepository contaCorrenteRepository)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
        }

        public async Task<AccountValidationResult> ValidateAccount(int? numeroConta)
        {

            if (!numeroConta.HasValue)
            {
                return AccountValidationResult.Invalid(
                    CreateErrorResponse(ValidationConstants.ERROR_INVALID_ACCOUNT, ValidationConstants.MSG_INVALID_ACCOUNT));
            }

            var contaCorrente = await _contaCorrenteRepository.GetByNumeroAsync(numeroConta.Value);

            if (contaCorrente == null)
            {
                return AccountValidationResult.Invalid(
                    CreateErrorResponse(ValidationConstants.ERROR_INVALID_ACCOUNT, ValidationConstants.MSG_INVALID_ACCOUNT));
            }

            if (!contaCorrente.Ativo)
            {
                return AccountValidationResult.Invalid(
                    CreateErrorResponse(ValidationConstants.ERROR_INACTIVE_ACCOUNT, ValidationConstants.MSG_INACTIVE_ACCOUNT));
            }

            return AccountValidationResult.Valid(contaCorrente);
        }

        private static MovimentarResponse CreateErrorResponse(string errorType, string message)
        {
            return new MovimentarResponse(false, errorType, message);
        }
    }
}
