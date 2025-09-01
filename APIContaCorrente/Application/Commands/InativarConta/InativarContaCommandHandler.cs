using APIContaCorrente.Application.Commands.CadastrarContaCorrente;
using APIContaCorrente.Application.Commands.Movimentar;
using APIContaCorrente.Application.Common.Constants;
using APIContaCorrente.Application.Queries.Saldo;
using APIContaCorrente.Application.Services;
using APIContaCorrente.Domain.Repositories;
using APIContaCorrente.Domain.Services;
using MediatR;

namespace APIContaCorrente.Application.Commands.InativarConta
{
    public class InativarContaCommandHandler : IRequestHandler<InativarContaCommand, InativarContaResponse>
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly IValidationService _validationService;

        public InativarContaCommandHandler(
            IContaCorrenteRepository contaCorrenteRepository,
            IPasswordHasherService passwordHasherService,
            IValidationService validationService)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
            _passwordHasherService = passwordHasherService;
            _validationService = validationService;
        }

        public async Task<InativarContaResponse> Handle(InativarContaCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var accountValidationResult = await _validationService.ValidateAccountByIdConta(request.ContaCorrenteId);
                if (!accountValidationResult.IsValid)
                {
                    return (InativarContaResponse)accountValidationResult.ErrorResponse;
                }

                var contaCorrente = accountValidationResult.ContaCorrente;

                if (!_passwordHasherService.VerifyPassword(request.Senha, contaCorrente.Senha, contaCorrente.Salt))
                {
                    return CreateErrorResponse(ValidationConstants.ERROR_USER_UNAUTHORIZED, ValidationConstants.MSG_INVALID_CREDENTIALS);
                }

                contaCorrente.Desativar();
                await _contaCorrenteRepository.UpdateAsync(contaCorrente);

                return CreateSuccessResponse();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(ValidationConstants.ERROR_INTERNAL_SERVER_ERROR, $"Erro interno: {ex.Message}");
            }
        }

        private static InativarContaResponse CreateErrorResponse(string errorType, string message)
        {
            return new InativarContaResponse(false, message, errorType);
        }

        private static InativarContaResponse CreateSuccessResponse()
        {
            return new InativarContaResponse(true);
        }
    }
}
