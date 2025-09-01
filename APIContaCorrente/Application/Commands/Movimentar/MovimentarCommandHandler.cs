using MediatR;
using APIContaCorrente.Domain.Entities;
using APIContaCorrente.Domain.Repositories;
using APIContaCorrente.Application.Common.Constants;
using APIContaCorrente.Application.Common.Validators;
using APIContaCorrente.Application.Services;

namespace APIContaCorrente.Application.Commands.Movimentar
{
    public class MovimentarCommandHandler : IRequestHandler<MovimentarCommand, MovimentarResponse>
    {
        private readonly IContaCorrenteRepository _contaCorrenteRepository;
        private readonly IMovimentoRepository _movimentoRepository;
        private readonly IIdempotenciaRepository _idempotenciaRepository;
        private readonly IValidationService _validationService;
        private readonly ILogger<MovimentarCommandHandler> _logger;

        public MovimentarCommandHandler(
            IContaCorrenteRepository contaCorrenteRepository,
            IMovimentoRepository movimentoRepository,
            IIdempotenciaRepository idempotenciaRepository,
            IValidationService validationService,
            ILogger<MovimentarCommandHandler> logger)
        {
            _contaCorrenteRepository = contaCorrenteRepository;
            _movimentoRepository = movimentoRepository;
            _idempotenciaRepository = idempotenciaRepository;
            _validationService = validationService;
            _logger = logger;
        }

        public async Task<MovimentarResponse> Handle(MovimentarCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (await IsAlreadyProcessed(request.IdRequisicao))
                {
                    return CreateSuccessResponse();
                }

                var valueValidation = ValidateValue(request.Valor);
                if (!valueValidation.IsValid)
                {
                    return valueValidation.ErrorResponse;
                }

                var typeValidation = await ValidateMovementTypeAsync(request);
                if (!typeValidation.IsValid)
                {
                    return typeValidation.ErrorResponse;
                }

                var accountValidationResult = await _validationService.ValidateAccountByNumeroConta(request.NumeroConta);
                if (!accountValidationResult.IsValid)
                {
                    return (MovimentarResponse)accountValidationResult.ErrorResponse;
                }

                var contaCorrente = accountValidationResult.ContaCorrente;

                await ProcessMovement(request, contaCorrente);

                return CreateSuccessResponse();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(ValidationConstants.ERROR_INTERNAL_SERVER_ERROR, $"Erro interno: {ex.Message}");
            }
        }

        private async Task<bool> IsAlreadyProcessed(string idRequisicao)
        {
            return await _idempotenciaRepository.ExistsAsync(idRequisicao);
        }

        private static ValidationResult<MovimentarResponse> ValidateValue(decimal valor)
        {
            if (valor <= 0)
            {
                return ValidationResult<MovimentarResponse>.Invalid(
                    CreateErrorResponse(ValidationConstants.ERROR_INVALID_VALUE, ValidationConstants.MSG_INVALID_VALUE));
            }

            return ValidationResult<MovimentarResponse>.Valid();
        }

        private async Task<ValidationResult<MovimentarResponse>> ValidateMovementTypeAsync(MovimentarCommand request)
        {
            if (!MovementTypeExists(request.TipoMovimento))
            {
                return ValidationResult<MovimentarResponse>.Invalid(
                    CreateErrorResponse(ValidationConstants.ERROR_INVALID_TYPE, ValidationConstants.MSG_INVALID_TYPE));
            }

            if (request.NumeroConta is not null)
            {
                if (await AttemptingDebitFromDifferentAccountAsync(request))
                {
                    return ValidationResult<MovimentarResponse>.Invalid(
                        CreateErrorResponse(ValidationConstants.ERROR_INVALID_TYPE, ValidationConstants.MSG_INVALID_TYPE));
                }
            }

            return ValidationResult<MovimentarResponse>.Valid();
        }

        private static bool MovementTypeExists(char tipoMovimento)
        {
            return tipoMovimento == ValidationConstants.TIPO_CREDITO || tipoMovimento == ValidationConstants.TIPO_DEBITO;
        }

        private async Task<bool> AttemptingDebitFromDifferentAccountAsync(MovimentarCommand request)
        {
            var contaCorrenteUsuario = await _contaCorrenteRepository.GetByIdAsync(request.ContaCorrenteId);

            return request.TipoMovimento == ValidationConstants.TIPO_DEBITO
                   && request.NumeroConta.HasValue
                   && request.NumeroConta.Value != contaCorrenteUsuario?.Numero;
        }

        private async Task ProcessMovement(MovimentarCommand request, ContaCorrente contaCorrente)
        {
            _logger.LogInformation("[Mov] Iniciando movimento: IdReq={IdReq}, Conta={Conta}, Tipo={Tipo}, Valor={Valor}", request.IdRequisicao, contaCorrente.IdContaCorrente, request.TipoMovimento, request.Valor);
            var movimento = CreateMovement(request, contaCorrente);
            await _movimentoRepository.AddAsync(movimento);
            _logger.LogInformation("[Mov] Movimento persistido: IdMov={IdMov}, Conta={Conta}, Tipo={Tipo}, Valor={Valor}", movimento.IdMovimento, movimento.IdContaCorrente, movimento.TipoMovimento, movimento.Valor);

            var idempotencia = CreateIdempotencia(request, movimento);
            await _idempotenciaRepository.AddAsync(idempotencia);
        }

        private static Movimento CreateMovement(MovimentarCommand request, ContaCorrente contaCorrente)
        {
            return new Movimento
            {
                IdContaCorrente = contaCorrente.IdContaCorrente,
                DataMovimento = DateTime.UtcNow,
                TipoMovimento = request.TipoMovimento,
                Valor = request.Valor
            };
        }

        private static Idempotencia CreateIdempotencia(MovimentarCommand request, Movimento movimento)
        {
            var requestData = new
            {
                request.Valor,
                request.TipoMovimento,
                NumeroConta = request.NumeroConta ?? 0
            };

            var resultData = new
            {
                movimento.IdMovimento,
                movimento.IdContaCorrente,
                movimento.DataMovimento,
                movimento.TipoMovimento,
                movimento.Valor
            };

            return new Idempotencia()
            {
                ChaveIdempotencia = request.IdRequisicao,
                Requisicao = System.Text.Json.JsonSerializer.Serialize(requestData),
                Resultado = System.Text.Json.JsonSerializer.Serialize(resultData)
            };
        }

        private static MovimentarResponse CreateErrorResponse(string errorType, string message)
        {
            return new MovimentarResponse(false, message, errorType);
        }

        private static MovimentarResponse CreateSuccessResponse()
        {
            return new MovimentarResponse(true);
        }
    }
}