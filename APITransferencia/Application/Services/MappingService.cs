using APITransferencia.Application.Commands.EfetuarTransferencia;
using APITransferencia.Application.DTOs;

namespace APITransferencia.Application.Services
{
    public class MappingService : IMappingService
    {
        public EfetuarTransferenciaCommand MapToCommand(EfetuarTransferenciaDto dto, string contaCorrenteIdOrigem, string? bearerToken)
        {
            return new EfetuarTransferenciaCommand
            {
                IdRequisicao = dto.IdRequisicao,
                NumeroContaDestino = dto.NumeroContaDestino,
                Valor = dto.Valor,
                ContaCorrenteIdOrigem = contaCorrenteIdOrigem,
                BearerToken = bearerToken
            };
        }
    }
}
