using MediatR;

namespace APITarifa.Application.Commands.CadastrarTarifa
{
    public class CadastrarTarifaCommand : IRequest<CadastrarTarifaResponse>
    {
        public string IdContaCorrente { get; set; } = string.Empty;
        public string DataMovimento { get; set; } = string.Empty;
        public decimal Valor { get; set; }
    }
}
