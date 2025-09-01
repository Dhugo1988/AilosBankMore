using MediatR;
using APITarifa.Domain.Entities;

namespace APITarifa.Application.Commands.ConsultarTarifaPorId
{
    public class ConsultarTarifaPorIdCommand : IRequest<ConsultarTarifaPorIdResponse>
    {
        public string IdTarifa { get; set; } = string.Empty;
    }
}
