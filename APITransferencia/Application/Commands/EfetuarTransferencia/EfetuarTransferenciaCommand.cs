using MediatR;

namespace APITransferencia.Application.Commands.EfetuarTransferencia
{
    public class EfetuarTransferenciaCommand : IRequest<EfetuarTransferenciaResponse>
    {
        public string IdRequisicao { get; set; } = string.Empty;
        public int NumeroContaDestino { get; set; }
        public decimal Valor { get; set; }
        public string ContaCorrenteIdOrigem { get; set; } = string.Empty;
        public string? BearerToken { get; set; }
    }
}
