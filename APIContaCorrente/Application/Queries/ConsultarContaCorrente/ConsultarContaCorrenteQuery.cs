using MediatR;

namespace APIContaCorrente.Application.Queries.ConsultarContaCorrente
{
    public class ConsultarContaCorrenteQuery : IRequest<ConsultarContaCorrenteResponse>
    {
        public string? Cpf { get; set; }
        public int? NumeroConta { get; set; }
    }
}
