namespace APITarifa.Infrastructure.Common.Constants
{
    public static class TarifaConstants
    {
        // Configurações
        public const string TARIFA_CONFIG_KEY = "Tarifa:ValorTransferencia";
        public const decimal TARIFA_DEFAULT_VALUE = 2.00m;
        public const string TARIFA_PREFIX = "tarifa-";
        
        // Formatos
        public const string DATE_FORMAT = "dd/MM/yyyy HH:mm:ss";
        
        // Mensagens de log
        public const string LOG_PROCESSING_STARTED = "Processando transferência realizada para conta {IdContaCorrente}";
        public const string LOG_ALREADY_PROCESSED = "Mensagem já processada: {IdRequisicao}";
        public const string LOG_SUCCESS = "Tarifa aplicada com sucesso. Conta: {IdContaCorrente}, Valor: {Valor}";
        public const string LOG_ERROR = "Erro ao processar transferência realizada para conta {IdContaCorrente}";
        
        // Operações
        public const char TIPO_DEBITO = 'D';
        public const string TARIFA_ID_PREFIX = "tarifa-";
    }
}
