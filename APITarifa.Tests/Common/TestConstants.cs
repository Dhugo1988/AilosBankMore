namespace APITarifa.Tests.Common
{
    public static class TestConstants
    {
        // Error Types para validação
        public const string ERROR_INVALID_CONFIGURATION = "INVALID_CONFIGURATION";
        public const string ERROR_TARIFA_CALCULATION_FAILED = "TARIFA_CALCULATION_FAILED";
        public const string ERROR_DATABASE_OPERATION_FAILED = "DATABASE_OPERATION_FAILED";
        public const string ERROR_MESSAGE_PRODUCER_FAILED = "MESSAGE_PRODUCER_FAILED";
        public const string ERROR_INVALID_MESSAGE = "INVALID_MESSAGE";
        public const string ERROR_INTERNAL_ERROR = "INTERNAL_ERROR";
        
        // Mensagens de erro para validação
        public const string MSG_INVALID_CONFIGURATION = "Configuração inválida";
        public const string MSG_TARIFA_CALCULATION_FAILED = "Falha no cálculo da tarifa";
        public const string MSG_DATABASE_OPERATION_FAILED = "Falha na operação do banco de dados";
        public const string MSG_MESSAGE_PRODUCER_FAILED = "Falha no produtor de mensagens";
        public const string MSG_INVALID_MESSAGE = "Mensagem inválida";
        public const string MSG_INTERNAL_ERROR = "Erro interno do servidor";
        
        // Mensagens de sucesso
        public const string MSG_TARIFA_PROCESSED = "Tarifa processada com sucesso";
        public const string MSG_ALREADY_PROCESSED = "Tarifa já processada";
        
        // Valores de teste
        public const string TEST_REQUEST_ID = "test-123";
        public const string TEST_ACCOUNT_ID = "12345678901";
        public const decimal TEST_TARIFA_VALUE = 2.00m;
        public const decimal TEST_DEFAULT_TARIFA_VALUE = 1.00m;
        public const string TEST_CONFIGURATION_SECTION = "Tarifa:ValorTransferencia";
        
        // Configurações de teste
        public const string TEST_CONFIG_KEY = "Tarifa:ValorTransferencia";
        public const string TEST_CONFIG_VALUE = "2.00";
    }
}
