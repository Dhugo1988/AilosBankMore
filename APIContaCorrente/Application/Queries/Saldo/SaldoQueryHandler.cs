using APIContaCorrente.Application.Common.Constants;
using APIContaCorrente.Application.Services;
using APIContaCorrente.Domain.Repositories;
using MediatR;

namespace APIContaCorrente.Application.Queries.Saldo
{
    public class SaldoQueryHandler : IRequestHandler<SaldoQuery, SaldoResponse>
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;
        private readonly IMovimentoRepository _movimentoRepository;
        private readonly IValidationService _validationService;

		public SaldoQueryHandler(
            IContaCorrenteRepository contaCorrenteRepository,
            IMovimentoRepository movimentoRepository,
            IValidationService validationService)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
            _movimentoRepository = movimentoRepository;
            _validationService = validationService;
		}

        public async Task<SaldoResponse> Handle(SaldoQuery request, CancellationToken cancellationToken)
        {
            try
            {
				var accountValidationResult = await _validationService.ValidateAccountByIdConta(request.ContaCorrenteId);
				if (!accountValidationResult.IsValid)
				{
					return (SaldoResponse)accountValidationResult.ErrorResponse;
				}

				var saldo = await _movimentoRepository.GetSaldoAsync(request.ContaCorrenteId);

                return CreateSuccessResponse(accountValidationResult.ContaCorrente.Numero,
                    accountValidationResult.ContaCorrente.Nome,
                    saldo);
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(ValidationConstants.ERROR_INTERNAL_SERVER_ERROR, $"Erro interno: {ex.Message}");
            }
        }

		private static SaldoResponse CreateErrorResponse(string errorType, string message)
		{
			return new SaldoResponse(false, message, errorType);
		}

		private static SaldoResponse CreateSuccessResponse(int? numeroConta, string? nomeTitular, decimal? saldo)
		{
			return new SaldoResponse(true, numeroConta,nomeTitular,DateTime.UtcNow,saldo);
		}
	}
}
