using MediatR;

namespace APIContaCorrente.Application.Commands.CadastrarContaCorrente
{
    public class CadastrarContaCorrenteCommand : IRequest<CadastrarContaCorrenteResponse>
    {
        public string Cpf { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
    }
}
