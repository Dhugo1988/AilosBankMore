using MediatR;

namespace APIContaCorrente.Application.Commands.Login
{
    public class LoginCommand : IRequest<LoginResponse>
    {
        public string Identificador { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }
}
