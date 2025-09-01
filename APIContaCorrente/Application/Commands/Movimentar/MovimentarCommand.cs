using MediatR;

namespace APIContaCorrente.Application.Commands.Movimentar
{
    public class MovimentarCommand : IRequest<MovimentarResponse>
    {
        public string ContaCorrenteId { get; set; } = string.Empty;
        public string IdRequisicao { get; set; } = string.Empty;
        public int? NumeroConta { get; set; }
        public decimal Valor { get; set; }
        public char TipoMovimento { get; set; }
    }
}
