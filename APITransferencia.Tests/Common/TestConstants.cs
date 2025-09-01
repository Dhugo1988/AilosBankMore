namespace APITransferencia.Tests.Common
{
    public static class TestConstants
    {
        // Error Types para validação
        public const string ERROR_INVALID_VALUE = "INVALID_VALUE";
        public const string ERROR_INVALID_ACCOUNT = "INVALID_ACCOUNT";
        public const string ERROR_INSUFFICIENT_BALANCE = "INSUFFICIENT_BALANCE";
        public const string ERROR_TRANSFER_FAILED = "TRANSFER_FAILED";
        public const string ERROR_DESTINATION_ACCOUNT_NOT_FOUND = "DESTINATION_ACCOUNT_NOT_FOUND";
        public const string ERROR_ORIGIN_ACCOUNT_NOT_FOUND = "ORIGIN_ACCOUNT_NOT_FOUND";
        public const string ERROR_INVALID_AMOUNT = "INVALID_AMOUNT";
        public const string ERROR_HTTP_REQUEST_FAILED = "HTTP_REQUEST_FAILED";
        public const string ERROR_INTERNAL_ERROR = "INTERNAL_ERROR";
        
        // Mensagens de erro para validação
        public const string MSG_INVALID_VALUE = "Valor deve ser maior que zero";
        public const string MSG_INVALID_ACCOUNT = "Conta inválida";
        public const string MSG_INSUFFICIENT_BALANCE = "Saldo insuficiente";
        public const string MSG_TRANSFER_FAILED = "Falha na transferência";
        public const string MSG_DESTINATION_ACCOUNT_NOT_FOUND = "Conta de destino não encontrada";
        public const string MSG_ORIGIN_ACCOUNT_NOT_FOUND = "Conta de origem não encontrada";
        public const string MSG_INVALID_AMOUNT = "Valor inválido para transferência";
        public const string MSG_HTTP_REQUEST_FAILED = "Falha na requisição HTTP";
        public const string MSG_INTERNAL_ERROR = "Erro interno do servidor";
        
        // Mensagens de sucesso
        public const string MSG_TRANSFER_SUCCESS = "Transferência realizada com sucesso";
        public const string MSG_ALREADY_PROCESSED = "Transferência já processada";
        
        // Valores de teste
        public const string TEST_REQUEST_ID = "test-123";
        public const string TEST_ACCOUNT_ID_ORIGEM = "12345678901";
        public const string TEST_ACCOUNT_ID_DESTINO = "98765432109";
        public const int TEST_ACCOUNT_NUMBER_DESTINO = 12345;
        public const decimal TEST_VALID_VALUE = 100;
        public const decimal TEST_INVALID_VALUE = -50;
        public const string TEST_BEARER_TOKEN = "Bearer token123";
        public const string TEST_HTTP_CLIENT_NAME = "ContaCorrenteAPI";
    }
}
