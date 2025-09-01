using MediatR;
using APITarifa.Application.DTOs;

namespace APITarifa.Application.Commands.ConsultarTarifasPorConta
{
    public class ConsultarTarifasPorContaCommand : IRequest<ConsultarTarifasPorContaResponse>
    {
        public string IdContaCorrente { get; set; } = string.Empty;
    }
}
