using MediatR;
using APITarifa.Domain.Repositories;
using APITarifa.Application.Common.Constants;

namespace APITarifa.Application.Commands.ConsultarTarifaPorId
{
    public class ConsultarTarifaPorIdCommandHandler : IRequestHandler<ConsultarTarifaPorIdCommand, ConsultarTarifaPorIdResponse>
    {
        private readonly ITarifaRepository _tarifaRepository;
        private readonly ILogger<ConsultarTarifaPorIdCommandHandler> _logger;

        public ConsultarTarifaPorIdCommandHandler(ITarifaRepository tarifaRepository, ILogger<ConsultarTarifaPorIdCommandHandler> logger)
        {
            _tarifaRepository = tarifaRepository;
            _logger = logger;
        }

        public async Task<ConsultarTarifaPorIdResponse> Handle(ConsultarTarifaPorIdCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.IdTarifa))
                {
                    return ConsultarTarifaPorIdResponse.CreateFailure(
                        ErrorConstants.ERROR_INVALID_TARIFA_ID,
                        MessageConstants.MSG_INVALID_TARIFA_ID
                    );
                }

                var tarifa = await _tarifaRepository.GetByIdAsync(request.IdTarifa);

                if (tarifa == null)
                {
                    return ConsultarTarifaPorIdResponse.CreateFailure(
                        ErrorConstants.ERROR_TARIFA_NOT_FOUND,
                        MessageConstants.MSG_TARIFA_NOT_FOUND
                    );
                }

                return ConsultarTarifaPorIdResponse.CreateSuccess(tarifa);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar tarifa {IdTarifa}", request.IdTarifa);
                return ConsultarTarifaPorIdResponse.CreateFailure(
                    ErrorConstants.ERROR_INTERNAL_SERVER,
                    MessageConstants.MSG_INTERNAL_SERVER_ERROR
                );
            }
        }
    }
}
