using APIContaCorrente.Application.Commands.Movimentar;
using APIContaCorrente.Application.Common.Constants;
using APIContaCorrente.Application.Common.Validators;
using APIContaCorrente.Domain.Entities;
using APIContaCorrente.Domain.Repositories;
using Microsoft.IdentityModel.Tokens;

namespace APIContaCorrente.Application.Services
{
    public class ValidationService : IValidationService
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;

        public ValidationService(IContaCorrenteRepository contaCorrenteRepository)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
        }

        public async Task<AccountValidationResult> ValidateAccountByIdConta(string idConta)
        {
            if (idConta.IsNullOrEmpty())
            {
                return AccountValidationResult.Invalid(
                    CreateErrorResponse(ValidationConstants.ERROR_INVALID_ACCOUNT, ValidationConstants.MSG_INVALID_ACCOUNT));
            }

            var contaCorrente = await _contaCorrenteRepository.GetByIdAsync(idConta);

            return ValidateAccount(contaCorrente);
        }

        public async Task<AccountValidationResult> ValidateAccountByNumeroConta(int? numeroConta)
        {
            if (!numeroConta.HasValue)
            {
                return AccountValidationResult.Invalid(
                    CreateErrorResponse(ValidationConstants.ERROR_INVALID_ACCOUNT, ValidationConstants.MSG_INVALID_ACCOUNT));
            }

            var contaCorrente = await _contaCorrenteRepository.GetByNumeroAsync(numeroConta.Value);

            return ValidateAccount(contaCorrente);
        }

        private static AccountValidationResult ValidateAccount(ContaCorrente? contaCorrente)
        {
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
