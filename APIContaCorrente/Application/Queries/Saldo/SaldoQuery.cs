using MediatR;

namespace APIContaCorrente.Application.Queries.Saldo
{
    public class SaldoQuery : IRequest<SaldoResponse>
    {
        public string ContaCorrenteId { get; set; } = string.Empty;
    }
}
