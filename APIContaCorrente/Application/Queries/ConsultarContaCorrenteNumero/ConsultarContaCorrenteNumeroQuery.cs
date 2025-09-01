using MediatR;

namespace APIContaCorrente.Application.Queries.ConsultarContaCorrente
{
    public class ConsultarContaCorrenteNumeroQuery : IRequest<ConsultarContaCorrenteNumeroResponse>
    {
        public string? ContaId { get; set; }
    }
}
