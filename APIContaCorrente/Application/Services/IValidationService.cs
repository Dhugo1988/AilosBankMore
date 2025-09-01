using APIContaCorrente.Application.Common.Validators;

namespace APIContaCorrente.Application.Services
{
    public interface IValidationService
    {
        Task<AccountValidationResult> ValidateAccountByNumeroConta(int? numeroConta);
        Task<AccountValidationResult> ValidateAccountByIdConta(string idConta);
    }
}
