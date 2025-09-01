using APIContaCorrente.Application.Common.Constants;
using APIContaCorrente.Domain.Repositories;
using MediatR;
using Microsoft.IdentityModel.Tokens;

namespace APIContaCorrente.Application.Queries.ConsultarContaCorrente
{
    public class ConsultarContaCorrenteNumeroQueryHandler : IRequestHandler<ConsultarContaCorrenteNumeroQuery, ConsultarContaCorrenteNumeroResponse>
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;

        public ConsultarContaCorrenteNumeroQueryHandler(
            IContaCorrenteRepository contaCorrenteRepository)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
        }

        public async Task<ConsultarContaCorrenteNumeroResponse> Handle(ConsultarContaCorrenteNumeroQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (!request.ContaId.IsNullOrEmpty())
                {
                    var contaPorId = await _contaCorrenteRepository.GetByIdAsync(request.ContaId!);
                    if (contaPorId != null)
                    {
                        return CreateSuccessResponse(contaPorId.Numero);
                    }
                }

                return CreateErrorResponse(ValidationConstants.ERROR_ACCOUNT_NOT_FOUND, ValidationConstants.MSG_ACCOUNT_NOT_FOUND);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(ValidationConstants.ERROR_INTERNAL_SERVER_ERROR, $"Erro interno: {ex.Message}");
            }
        }

        private static ConsultarContaCorrenteNumeroResponse CreateErrorResponse(string errorType, string message)
        {
            return new ConsultarContaCorrenteNumeroResponse(false, message, errorType);
        }

        private static ConsultarContaCorrenteNumeroResponse CreateSuccessResponse(int? numeroConta)
        {
            return new ConsultarContaCorrenteNumeroResponse(true, numeroConta);
        }
    }
}
