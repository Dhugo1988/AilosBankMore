using APIContaCorrente.Application.Common.Constants;
using APIContaCorrente.Domain.Entities;
using APIContaCorrente.Domain.Repositories;
using APIContaCorrente.Domain.Services;
using MediatR;

namespace APIContaCorrente.Application.Commands.CadastrarContaCorrente
{
    public class CadastrarContaCorrenteCommandHandler : IRequestHandler<CadastrarContaCorrenteCommand, CadastrarContaCorrenteResponse>
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;
        private readonly ICpfValidationService _cpfValidationService;
        private readonly IContaCorrenteService _contaCorrenteService;
        private readonly IPasswordHasherService _passwordHasherService;

        public CadastrarContaCorrenteCommandHandler(
            IContaCorrenteRepository contaCorrenteRepository,
            ICpfValidationService cpfValidationService,
            IContaCorrenteService contaCorrenteService,
            IPasswordHasherService passwordHasherService)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
            _cpfValidationService = cpfValidationService;
            _contaCorrenteService = contaCorrenteService;
            _passwordHasherService = passwordHasherService;
        }

        public async Task<CadastrarContaCorrenteResponse> Handle(CadastrarContaCorrenteCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Cpf))
                {
                    return CreateErrorResponse(ValidationConstants.ERROR_INVALID_DOCUMENT, ValidationConstants.MSG_INVALID_CPF);
                }

                if (!_cpfValidationService.IsValid(request.Cpf))
                {
                    return CreateErrorResponse(ValidationConstants.ERROR_INVALID_DOCUMENT, ValidationConstants.MSG_INVALID_CPF);
                }

                if (string.IsNullOrEmpty(request.Nome))
                {
                    return CreateErrorResponse(ValidationConstants.ERROR_INVALID_DOCUMENT, ValidationConstants.MSG_REQUIRED_NAME);
                }

                if (string.IsNullOrEmpty(request.Senha))
                {
                    return CreateErrorResponse(ValidationConstants.ERROR_INVALID_DOCUMENT, ValidationConstants.MSG_REQUIRED_PASSWORD);
                }

                if (await _contaCorrenteRepository.ExistsByCpfAsync(request.Cpf))
                {
                    return CreateErrorResponse(ValidationConstants.ERROR_INVALID_DOCUMENT, ValidationConstants.MSG_INVALID_ACCOUNT);
                }

                var numeroConta = _contaCorrenteService.GerarNumeroConta();

                var senhaHash = _passwordHasherService.HashPassword(request.Senha, out var salt);

                var contaCorrente = new ContaCorrente
                {
                    IdContaCorrente = request.Cpf,
                    Numero = numeroConta,
                    Nome = request.Nome,
                    Senha = senhaHash,
                    Salt = salt,
                    Ativo = true
                };

                await _contaCorrenteRepository.AddAsync(contaCorrente);

                return CreateSuccessResponse(numeroConta);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(ValidationConstants.ERROR_INTERNAL_SERVER_ERROR, $"Erro interno: {ex.Message}");
            }
        }

        private static CadastrarContaCorrenteResponse CreateErrorResponse(string errorType, string message)
        {
            return new CadastrarContaCorrenteResponse(false, message, errorType);
        }

        private static CadastrarContaCorrenteResponse CreateSuccessResponse(int? numeroConta)
        {
            return new CadastrarContaCorrenteResponse(true, numeroConta);
        }
    }
}
