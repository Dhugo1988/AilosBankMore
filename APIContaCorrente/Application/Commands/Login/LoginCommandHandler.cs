using APIContaCorrente.Application.Common.Constants;
using APIContaCorrente.Domain.Repositories;
using APIContaCorrente.Domain.Services;
using APIContaCorrente.Infrastructure.Security;
using MediatR;

namespace APIContaCorrente.Application.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly JwtTokenService _jwtTokenService;

        public LoginCommandHandler(
            IContaCorrenteRepository contaCorrenteRepository,
            IPasswordHasherService passwordHasherService,
            JwtTokenService jwtTokenService)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
            _passwordHasherService = passwordHasherService;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var contaCorrente = await _contaCorrenteRepository.GetByCpfAsync(request.Identificador);
                
                if (contaCorrente == null)
                {
                    if (int.TryParse(request.Identificador, out var numeroConta))
                    {
                        contaCorrente = await _contaCorrenteRepository.GetByNumeroAsync(numeroConta);
                    }
                }

                if (contaCorrente == null)
                {
                    return CreateErrorResponse(ValidationConstants.ERROR_USER_UNAUTHORIZED, ValidationConstants.MSG_USER_INVALID_CREDENTIALS);
                }

                // Verificar se a conta está ativa
                if (!contaCorrente.Ativo)
                {
                    return CreateErrorResponse(ValidationConstants.ERROR_USER_UNAUTHORIZED, ValidationConstants.MSG_USER_INVALID_CREDENTIALS);

                }

                // Verificar senha
                if (!_passwordHasherService.VerifyPassword(request.Senha, contaCorrente.Senha, contaCorrente.Salt))
                {
                    return CreateErrorResponse(ValidationConstants.ERROR_USER_UNAUTHORIZED, ValidationConstants.MSG_USER_INVALID_CREDENTIALS);
                }

                // Gerar token JWT
                var token = _jwtTokenService.GenerateToken(contaCorrente);

                return CreateSuccessResponse(token);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(ValidationConstants.ERROR_INTERNAL_SERVER_ERROR, $"Erro interno: {ex.Message}");
            }
        }

        private static LoginResponse CreateErrorResponse(string errorType, string message)
        {
            return new LoginResponse(false, message, errorType);
        }

        private static LoginResponse CreateSuccessResponse(string? token)
        {
            return new LoginResponse(true, token);
        }
    }
}
