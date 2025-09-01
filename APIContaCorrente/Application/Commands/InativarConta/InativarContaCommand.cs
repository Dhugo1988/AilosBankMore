using MediatR;

namespace APIContaCorrente.Application.Commands.InativarConta
{
    public class InativarContaCommand : IRequest<InativarContaResponse>
    {
        public string ContaCorrenteId { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }
}
