namespace APIContaCorrente.Application.Common.Constants
{
    public static class ValidationConstants
    {
        // Tipos de movimento
        public const char TIPO_CREDITO = 'C';
        public const char TIPO_DEBITO = 'D';
        
        // Tipos de erro
        public const string ERROR_INVALID_ACCOUNT = "INVALID_ACCOUNT";
        public const string ERROR_INACTIVE_ACCOUNT = "INACTIVE_ACCOUNT";
        public const string ERROR_INVALID_VALUE = "INVALID_VALUE";
        public const string ERROR_INVALID_TYPE = "INVALID_TYPE";
        public const string ERROR_INVALID_DOCUMENT = "INVALID_DOCUMENT";
        public const string ERROR_DUPLICATE_DOCUMENT = "DUPLICATE_DOCUMENT";
        public const string ERROR_USER_UNAUTHORIZED = "USER_UNAUTHORIZED";
        public const string ERROR_ACCOUNT_NOT_FOUND = "ACCOUNT_NOT_FOUND";
        public const string ERROR_INVALID_CREDENTIALS = "INVALID_CREDENTIALS";
        public const string ERROR_INTERNAL_SERVER_ERROR = "INTERNAL_SERVER_ERROR";

        // Mensagens de erro
        public const string MSG_ACCOUNT_NOT_FOUND = "Conta não encontrada";
        public const string MSG_INACTIVE_ACCOUNT = "Conta inativa";
        public const string MSG_SELF_CREDIT_ERROR = "Crédito não pode ser feito na própria conta";
        public const string MSG_INVALID_CPF = "CPF inválido";
        public const string MSG_REQUIRED_NAME = "Nome é obrigatório";
        public const string MSG_REQUIRED_PASSWORD = "Senha é obrigatória";
        public const string MSG_DUPLICATE_CPF = "CPF já cadastrado";
        public const string MSG_USER_INVALID_CREDENTIALS = "Credenciais inválidas";
        public const string MSG_INCORRECT_PASSWORD = "Senha incorreta";
        public const string MSG_ALREADY_INACTIVE = "Conta já está inativa";
        public const string MSG_INVALID_ACCOUNT = "Conta inválida";
        public const string MSG_INVALID_VALUE = "Valor inválido";
        public const string MSG_INVALID_TYPE = "Tipo de movimento inválido";
        public const string MSG_INVALID_CREDENTIALS = "Credenciais inválidas";
        public const string MSG_INTERNAL_SERVER_ERROR = "Erro interno do servidor";
    }
}
