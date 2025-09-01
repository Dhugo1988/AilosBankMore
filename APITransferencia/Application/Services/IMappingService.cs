using APITransferencia.Application.Commands.EfetuarTransferencia;
using APITransferencia.Application.DTOs;

namespace APITransferencia.Application.Services
{
    public interface IMappingService
    {
        EfetuarTransferenciaCommand MapToCommand(EfetuarTransferenciaDto dto, string contaCorrenteIdOrigem, string? bearerToken);
    }
}
