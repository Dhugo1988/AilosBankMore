using APITarifa.Domain.Entities;
using APITarifa.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace APITarifa.Application.Commands.CadastrarTarifa
{
    public class CadastrarTarifaCommandHandler : IRequestHandler<CadastrarTarifaCommand, CadastrarTarifaResponse>
    {
        private readonly ITarifaRepository _tarifaRepository;
        private readonly ILogger<CadastrarTarifaCommandHandler> _logger;

        public CadastrarTarifaCommandHandler(
            ITarifaRepository tarifaRepository,
            ILogger<CadastrarTarifaCommandHandler> logger)
        {
            _tarifaRepository = tarifaRepository;
            _logger = logger;
        }

        public async Task<CadastrarTarifaResponse> Handle(CadastrarTarifaCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validações básicas
                if (string.IsNullOrEmpty(request.IdContaCorrente))
                {
                    return new CadastrarTarifaResponse(false, "ID da conta corrente é obrigatório", "BAD_REQUEST");
                }

                if (string.IsNullOrEmpty(request.DataMovimento))
                {
                    return new CadastrarTarifaResponse(false, "Data do movimento é obrigatória", "BAD_REQUEST");
                }

                if (request.Valor <= 0)
                {
                    return new CadastrarTarifaResponse(false, "Valor deve ser maior que zero", "BAD_REQUEST");
                }

                // Criar a tarifa
                var tarifa = new Tarifa
                {
                    IdContaCorrente = request.IdContaCorrente,
                    DataMovimento = request.DataMovimento,
                    Valor = request.Valor
                };

                // Salvar na base de dados
                await _tarifaRepository.AddAsync(tarifa);

                _logger.LogInformation("Tarifa cadastrada com sucesso. ID: {TarifaId}, Conta: {ContaCorrente}, Valor: {Valor}", 
                    tarifa.IdTarifa, tarifa.IdContaCorrente, tarifa.Valor);

                return new CadastrarTarifaResponse(
                    true, 
                    tarifa.IdTarifa, 
                    tarifa.IdContaCorrente, 
                    tarifa.DataMovimento, 
                    tarifa.Valor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cadastrar tarifa para conta {ContaCorrente}", request.IdContaCorrente);
                return new CadastrarTarifaResponse(false, "Erro interno ao cadastrar tarifa", "INTERNAL_ERROR");
            }
        }
    }
}
