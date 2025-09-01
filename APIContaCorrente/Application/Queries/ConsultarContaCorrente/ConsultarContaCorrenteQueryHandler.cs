using APIContaCorrente.Application.Common.Constants;
using APIContaCorrente.Domain.Repositories;
using MediatR;

namespace APIContaCorrente.Application.Queries.ConsultarContaCorrente
{
    public class ConsultarContaCorrenteQueryHandler : IRequestHandler<ConsultarContaCorrenteQuery, ConsultarContaCorrenteResponse>
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;

        public ConsultarContaCorrenteQueryHandler(
            IContaCorrenteRepository contaCorrenteRepository)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
        }

        public async Task<ConsultarContaCorrenteResponse> Handle(ConsultarContaCorrenteQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (!string.IsNullOrEmpty(request.Cpf))
                {
                    var contaPorCpf = await _contaCorrenteRepository.GetByCpfAsync(request.Cpf);
                    if (contaPorCpf != null)
                    {
                        return CreateSuccessResponse(contaPorCpf.IdContaCorrente);
                    }
                }

                if (request.NumeroConta.HasValue)
                {
                    var contaPorNumero = await _contaCorrenteRepository.GetByNumeroAsync(request.NumeroConta.Value);
                    if (contaPorNumero != null)
                    {
                        return CreateSuccessResponse(contaPorNumero.IdContaCorrente);
                    }
                }

                return CreateErrorResponse(ValidationConstants.ERROR_ACCOUNT_NOT_FOUND, ValidationConstants.MSG_ACCOUNT_NOT_FOUND);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(ValidationConstants.ERROR_INTERNAL_SERVER_ERROR, $"Erro interno: {ex.Message}");
            }
        }

        private static ConsultarContaCorrenteResponse CreateErrorResponse(string errorType, string message)
        {
            return new ConsultarContaCorrenteResponse(false, message, errorType);
        }

        private static ConsultarContaCorrenteResponse CreateSuccessResponse(string contaId)
        {
            return new ConsultarContaCorrenteResponse(true, contaId);
        }
    }
}
