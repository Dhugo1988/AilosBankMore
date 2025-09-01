using APITransferencia.Application.DTOs;
using APITransferencia.Domain.Entities;
using APITransferencia.Infrastructure.Data;
using APITransferencia.Infrastructure.Messaging.Messages;
using APITransferencia.Infrastructure.Messaging.Producers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace APITransferencia.Application.Commands.EfetuarTransferencia
{
    public class EfetuarTransferenciaCommandHandler : IRequestHandler<EfetuarTransferenciaCommand, EfetuarTransferenciaResponse>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITransferenciaDbContext _db;
        private readonly ITransferenciaProducer _transferenciaProducer;
        private readonly ILogger<EfetuarTransferenciaCommandHandler> _logger;

        // Constantes para tipos de erro
        private const string ERROR_INVALID_ACCOUNT = "INVALID_ACCOUNT";
        private const string ERROR_BAD_REQUEST = "BAD_REQUEST";
        private const string ERROR_INTERNAL_ERROR = "INTERNAL_ERROR";

        // Constantes para mensagens
        private const string MSG_DEBIT_FAILED = "Falha no débito na origem";
        private const string MSG_CREDIT_FAILED = "Falha no crédito no destino";

        // Constantes para operações
        private const char TIPO_DEBITO = 'D';
        private const char TIPO_CREDITO = 'C';
        private const string CONTA_CORRENTE_CLIENT = "ContaCorrente";
        private const string CONTENT_TYPE_JSON = "application/json";
        
        // Constantes para URLs
        private const string CONTA_NUMERO_QUERY_URL_TEMPLATE = "api/contacorrente/por-id?contaId={0}";
        private const string CONTA_ID_QUERY_URL_TEMPLATE = "api/contacorrente/por-numero?numeroConta={0}";
        private const string MOVIMENTAR_URL = "api/contacorrente/movimentar";

        public EfetuarTransferenciaCommandHandler(
            IHttpClientFactory httpClientFactory, 
            ITransferenciaDbContext db,
            ITransferenciaProducer transferenciaProducer,
            ILogger<EfetuarTransferenciaCommandHandler> logger)
        {
            _httpClientFactory = httpClientFactory;
            _db = db;
            _transferenciaProducer = transferenciaProducer;
            _logger = logger;
        }

        public async Task<EfetuarTransferenciaResponse> Handle(EfetuarTransferenciaCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validações básicas
                if (string.IsNullOrEmpty(request.IdRequisicao))
                {
                    return CreateErrorResponse("ID da requisição é obrigatório", ERROR_BAD_REQUEST);
                }

                if (string.IsNullOrEmpty(request.ContaCorrenteIdOrigem))
                {
                    return CreateErrorResponse("Conta origem inválida", ERROR_BAD_REQUEST);
                }

                if (request.NumeroContaDestino <= 0)
                {
                    return CreateErrorResponse("Conta destino inválida", ERROR_BAD_REQUEST);
                }

                if (request.Valor <= 0)
                {
                    return CreateErrorResponse("Valor deve ser maior que zero", ERROR_BAD_REQUEST);
                }

                if (await IsAlreadyProcessed(request.IdRequisicao, cancellationToken))
                {
                    return CreateSuccessResponse();
                }

                var client = ConfigureHttpClient(request.BearerToken);

                var buscaContaIdDestinoResult = await BuscaIdConta(client, request.NumeroContaDestino, cancellationToken);
                if (!buscaContaIdDestinoResult.IsValid)
                {
                    return CreateErrorResponse(buscaContaIdDestinoResult.ErrorResponse.ErrorType!, buscaContaIdDestinoResult.ErrorResponse.Message!);
                }

                var buscaContaOrigemResult = await BuscaNumeroConta(client, request.ContaCorrenteIdOrigem, cancellationToken);
                if (!buscaContaOrigemResult.IsValid)
                {
                    return CreateErrorResponse(buscaContaOrigemResult.ErrorResponse.ErrorType!, buscaContaOrigemResult.ErrorResponse.Message!);
                }

                var transferResult = await ExecuteTransfer(client, request,buscaContaOrigemResult.NumeroConta, cancellationToken);
                if (!transferResult.Success)
                {
                    return transferResult;
                }

                await PersistTransferData(request, buscaContaIdDestinoResult.ContaCorrenteId, cancellationToken);

                await NotifyTransferCompleted(request);

                return CreateSuccessResponse();
            }
            catch (Exception ex)
            {
                return CreateErrorResponse(ERROR_INTERNAL_ERROR, $"Erro interno: {ex.Message}");
            }
        }

        private async Task<bool> IsAlreadyProcessed(string idRequisicao, CancellationToken cancellationToken)
        {
            var idem = await _db.Idempotencias.FirstOrDefaultAsync(i => i.ChaveIdempotencia == idRequisicao, cancellationToken);
            return idem != null;
        }

        private HttpClient ConfigureHttpClient(string? bearerToken)
        {
            var client = _httpClientFactory.CreateClient(CONTA_CORRENTE_CLIENT);
            
            if (!string.IsNullOrWhiteSpace(bearerToken))
            {
                try
                {
                    var token = bearerToken.Trim();
                    if (token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                    {
                        token = token.Substring("Bearer ".Length).Trim();
                    }
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }
                catch
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", bearerToken);
                }
            }

            return client;
        }
        private async Task<EfetuarTransferenciaResponse> ExecuteTransfer(HttpClient client, EfetuarTransferenciaCommand request, int? numeroConta,CancellationToken cancellationToken)
        {
            // Executar débito na origem
            var debitResult = await ExecuteDebit(client, request, numeroConta, cancellationToken);
            
            if (!debitResult.Success)
            {
                return debitResult;
            }

            // Executar crédito no destino
            var creditResult = await ExecuteCredit(client, request, cancellationToken);
            if (!creditResult.Success)
            {
                // Executar estorno em caso de falha no crédito
                await ExecuteRollback(client, request, cancellationToken);
                return creditResult;
            }

            return CreateSuccessResponse();
        }

        private async Task<BuscaContaNumeroResult> BuscaNumeroConta(HttpClient client, string contaId, CancellationToken cancellationToken)
        {
            try
            {
                var url = string.Format(client.BaseAddress + CONTA_NUMERO_QUERY_URL_TEMPLATE, contaId);

                var response = await client.GetAsync(url, cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    return BuscaContaNumeroResult.Invalid(
                        CreateErrorResponse(ERROR_INVALID_ACCOUNT, "Conta não encontrada"));
                }

                var accountResponse = JsonSerializer.Deserialize<ConsultarContaCorrenteNumeroResponse>(content);
                if (accountResponse?.NumeroConta == null)
                {
                    return BuscaContaNumeroResult.Invalid(
                        CreateErrorResponse(ERROR_INVALID_ACCOUNT, "Conta inválida"));
                }

                return BuscaContaNumeroResult.Valid(accountResponse.NumeroConta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar conta {contaId}", contaId);
                throw;
            }
        }

        private async Task<BuscaContaIdResult> BuscaIdConta(HttpClient client, int numeroConta, CancellationToken cancellationToken)
        {
            try
            {
                var url = string.Format(client.BaseAddress + CONTA_ID_QUERY_URL_TEMPLATE, numeroConta);

                var response = await client.GetAsync(url, cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    return BuscaContaIdResult.Invalid(
                        CreateErrorResponse(ERROR_INVALID_ACCOUNT, "Conta não encontrada"));
                }

                var accountResponse = JsonSerializer.Deserialize<ConsultarContaCorrenteIdResponse>(content);
                if (accountResponse?.ContaCorrenteId == null)
                {
                    return BuscaContaIdResult.Invalid(
                        CreateErrorResponse(ERROR_INVALID_ACCOUNT, "Conta inválida"));
                }

                return BuscaContaIdResult.Valid(accountResponse.ContaCorrenteId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consultar conta {numeroConta}", numeroConta);
                throw;
            }
        }

        private async Task<EfetuarTransferenciaResponse> ExecuteDebit(HttpClient client, EfetuarTransferenciaCommand request, int? numeroConta, CancellationToken cancellationToken)
        {
            var debitoPayload = CreateMovementPayload(request.Valor, TIPO_DEBITO, numeroConta);
            var debitResp = await client.PostAsync(client.BaseAddress + MOVIMENTAR_URL, CreateJsonContent(debitoPayload), cancellationToken);
            
            if (!debitResp.IsSuccessStatusCode)
            {
                var responseContent = await debitResp.Content.ReadAsStringAsync();
                var movimentarResponse = JsonSerializer.Deserialize<MovimentarResponseDto>(responseContent);

                string errorType = movimentarResponse?.ErrorType ?? ERROR_BAD_REQUEST;
                string message = movimentarResponse?.Message ?? MSG_DEBIT_FAILED;
                _logger.LogWarning("Falha no débito: Status={Status} ErrorType={ErrorType} Body={Body}", debitResp.StatusCode, errorType, responseContent);

                return CreateErrorResponse(errorType, message);
            }

            return CreateSuccessResponse();
        }

        private async Task<EfetuarTransferenciaResponse> ExecuteCredit(HttpClient client, EfetuarTransferenciaCommand request, CancellationToken cancellationToken)
        {
            var creditoPayload = CreateMovementPayload( 
                request.Valor, 
                TIPO_CREDITO, 
                request.NumeroContaDestino);
                
            var creditResp = await client.PostAsync(client.BaseAddress + MOVIMENTAR_URL, CreateJsonContent(creditoPayload), cancellationToken);
            
            if (!creditResp.IsSuccessStatusCode)
            {
                var responseContent = await creditResp.Content.ReadAsStringAsync();
                var movimentarResponse = JsonSerializer.Deserialize<MovimentarResponseDto>(responseContent);

                string errorType = movimentarResponse?.ErrorType ?? ERROR_BAD_REQUEST;
                string message = movimentarResponse?.Message ?? MSG_CREDIT_FAILED;

                return CreateErrorResponse(errorType, message);
            }

            return CreateSuccessResponse();
        }

        private async Task<EfetuarTransferenciaResponse> ExecuteRollback(HttpClient client, EfetuarTransferenciaCommand request, CancellationToken cancellationToken)
        {
            var estornoPayload = CreateMovementPayload(request.Valor, TIPO_DEBITO);
            var estornoResp = await client.PostAsync(MOVIMENTAR_URL, CreateJsonContent(estornoPayload), cancellationToken);

            if (!estornoResp.IsSuccessStatusCode)
            {
                var responseContent = await estornoResp.Content.ReadAsStringAsync();
                var movimentarResponse = JsonSerializer.Deserialize<MovimentarResponseDto>(responseContent);

                string errorType = movimentarResponse?.ErrorType ?? ERROR_BAD_REQUEST;
                string message = movimentarResponse?.Message ?? MSG_CREDIT_FAILED;

                return CreateErrorResponse(errorType, message);
            }

            return CreateSuccessResponse();
        }

        private static string CreateMovementPayload(decimal valor, char tipoMovimento, int? numeroConta = null)
        {
            var payload = new MovimentarRequestDto
            {
                IdRequisicao = Guid.NewGuid().ToString(),
                Valor = valor,
                TipoMovimento = tipoMovimento,
                NumeroConta = numeroConta
            };

            return JsonSerializer.Serialize(payload);
        }

        private static StringContent CreateJsonContent(string payload)
        {
            return new StringContent(payload, Encoding.UTF8, CONTENT_TYPE_JSON);
        }

        private async Task PersistTransferData(EfetuarTransferenciaCommand request, string contaDestinoId, CancellationToken cancellationToken)
        {
            var transferencia = CreateTransferencia(request, contaDestinoId);
            var idempotencia = CreateIdempotencia(request, transferencia);

            _db.Transferencias.Add(transferencia);
            _db.Idempotencias.Add(idempotencia);
            
            await _db.SaveChangesAsync(cancellationToken);
        }

        private static Transferencia CreateTransferencia(EfetuarTransferenciaCommand request, string contaDestinoId)
        {
            return new Transferencia(
                request.ContaCorrenteIdOrigem, 
                contaDestinoId, 
                DateTime.UtcNow.ToString("dd/MM/yyyy"), 
                request.Valor);
        }

        private static Idempotencia CreateIdempotencia(EfetuarTransferenciaCommand request, Transferencia transferencia)
        {
            var requestData = new
            {
                request.Valor,
                request.NumeroContaDestino,
                request.ContaCorrenteIdOrigem
            };

            var resultData = new
            {
                transferencia.IdTransferencia,
                transferencia.IdContaCorrenteDestino,
                transferencia.IdContaCorrenteOrigem,
                transferencia.Valor,
                transferencia.DataMovimento
            };

            return new Idempotencia()
            {
                Requisicao = System.Text.Json.JsonSerializer.Serialize(requestData),
                Resultado = System.Text.Json.JsonSerializer.Serialize(resultData)
            };
        }

        private async Task NotifyTransferCompleted(EfetuarTransferenciaCommand request)
        {
            var transferenciaMessage = new TransferenciaRealizadaMessage
            {
                IdRequisicao = Guid.NewGuid().ToString(),
                IdContaCorrente = request.ContaCorrenteIdOrigem,
                DataMovimento = DateTime.UtcNow.ToString()
            };

            await _transferenciaProducer.TransferAsync(transferenciaMessage);
        }

        private static EfetuarTransferenciaResponse CreateErrorResponse(string errorType, string message)
        {
            return new EfetuarTransferenciaResponse(false, errorType, message);
        }

        private static EfetuarTransferenciaResponse CreateSuccessResponse()
        {
            return new EfetuarTransferenciaResponse(true);
        }

        private class BuscaContaNumeroResult
        {
            public bool IsValid { get; private set; }
            public int? NumeroConta { get; private set; }
            public EfetuarTransferenciaResponse ErrorResponse { get; private set; }

            protected BuscaContaNumeroResult(bool isValid, int? numeroConta = null, EfetuarTransferenciaResponse errorResponse = null)
            {
                IsValid = isValid;
                NumeroConta = numeroConta;
                ErrorResponse = errorResponse;
            }
            public static BuscaContaNumeroResult Valid(int? numeroConta) => new(true, numeroConta);
            public static BuscaContaNumeroResult Invalid(EfetuarTransferenciaResponse errorResponse) => new(false, null, errorResponse);
        }

        private class BuscaContaIdResult
        {
            public bool IsValid { get; private set; }
            public string ContaCorrenteId { get; private set; }
            public EfetuarTransferenciaResponse ErrorResponse { get; private set; }

            protected BuscaContaIdResult(bool isValid, string contaCorrenteId = null, EfetuarTransferenciaResponse errorResponse = null)
            {
                IsValid = isValid;
                ContaCorrenteId = contaCorrenteId;
                ErrorResponse = errorResponse;
            }
            public static BuscaContaIdResult Valid(string contaCorrenteId) => new(true, contaCorrenteId);
            public static BuscaContaIdResult Invalid(EfetuarTransferenciaResponse errorResponse) => new(false, null, errorResponse);
        }
    }
}
