using MediatR;
using APITarifa.Domain.Repositories;
using APITarifa.Application.DTOs;
using APITarifa.Application.Common.Constants;

namespace APITarifa.Application.Commands.ConsultarTarifasPorConta
{
    public class ConsultarTarifasPorContaCommandHandler : IRequestHandler<ConsultarTarifasPorContaCommand, ConsultarTarifasPorContaResponse>
    {
        private readonly ITarifaRepository _tarifaRepository;
        private readonly ILogger<ConsultarTarifasPorContaCommandHandler> _logger;

        public ConsultarTarifasPorContaCommandHandler(ITarifaRepository tarifaRepository, ILogger<ConsultarTarifasPorContaCommandHandler> logger)
        {
            _tarifaRepository = tarifaRepository;
            _logger = logger;
        }

        public async Task<ConsultarTarifasPorContaResponse> Handle(ConsultarTarifasPorContaCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Consultando tarifas para conta {IdContaCorrente}", request.IdContaCorrente);

                if (string.IsNullOrWhiteSpace(request.IdContaCorrente))
                {
                    return ConsultarTarifasPorContaResponse.CreateFailure(
                        ErrorConstants.ERROR_INVALID_ACCOUNT_ID,
                        MessageConstants.MSG_INVALID_ACCOUNT_ID
                    );
                }

                var tarifas = await _tarifaRepository.GetByContaCorrenteAsync(request.IdContaCorrente);

                var response = new TarifaResponseDto
                {
                    IdContaCorrente = request.IdContaCorrente,
                    TotalTarifas = tarifas.Count,
                    ValorTotalTarifado = tarifas.Sum(t => t.Valor),
                    Tarifas = tarifas.Select(t => new TarifaItemDto
                    {
                        IdTarifa = t.IdTarifa,
                        DataMovimento = DateTime.ParseExact(t.DataMovimento, "dd/MM/yyyy HH:mm:ss", null),
                        Valor = t.Valor
                    }).ToList()
                };

                return ConsultarTarifasPorContaResponse.CreateSuccess(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar tarifas para conta {IdContaCorrente}", request.IdContaCorrente);
                return ConsultarTarifasPorContaResponse.CreateFailure(
                    ErrorConstants.ERROR_INTERNAL_SERVER,
                    MessageConstants.MSG_INTERNAL_SERVER_ERROR
                );
                }
            }
        }
    }
