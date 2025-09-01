namespace APIContaCorrente.Tests.Common
{
    public static class TestConstants
    {
        // Error Types para validação
        public const string ERROR_INVALID_ACCOUNT = "ACCOUNT_NOT_FOUND";
        public const string ERROR_INACTIVE_ACCOUNT = "ACCOUNT_INACTIVE";
        public const string ERROR_INVALID_VALUE = "INVALID_VALUE";
        public const string ERROR_INVALID_TYPE = "INVALID_MOVEMENT_TYPE";
        public const string ERROR_INVALID_CREDIT_TO_SELF = "INVALID_CREDIT_TO_SELF";
        public const string ERROR_INVALID_DOCUMENT = "INVALID_DOCUMENT";
        public const string ERROR_DUPLICATE_DOCUMENT = "DUPLICATE_DOCUMENT";
        public const string ERROR_USER_UNAUTHORIZED = "USER_UNAUTHORIZED";
        public const string ERROR_INTERNAL_ERROR = "INTERNAL_ERROR";
        
        // Mensagens de erro para validação
        public const string MSG_ACCOUNT_NOT_FOUND = "Conta não encontrada";
        public const string MSG_INACTIVE_ACCOUNT = "Conta inativa";
        public const string MSG_INVALID_VALUE = "Valor deve ser maior que zero";
        public const string MSG_INVALID_MOVEMENT_TYPE = "Tipo de movimento deve ser 'C' (Crédito) ou 'D' (Débito)";
        public const string MSG_SELF_CREDIT_ERROR = "Crédito não pode ser feito na própria conta";
        public const string MSG_ALREADY_PROCESSED = "Requisição já processada";
        public const string MSG_INVALID_CPF = "CPF inválido";
        public const string MSG_DUPLICATE_CPF = "CPF já cadastrado";
        public const string MSG_USER_NOT_FOUND = "Usuário não encontrado";
        public const string MSG_INCORRECT_PASSWORD = "Senha incorreta";
        public const string MSG_ALREADY_INACTIVE = "Conta já está inativa";
        
        // Mensagens de sucesso
        public const string MSG_MOVEMENT_SUCCESS = "Movimentação realizada com sucesso";
        public const string MSG_ACCOUNT_CREATED = "Conta criada com sucesso";
        public const string MSG_LOGIN_SUCCESS = "Login realizado com sucesso";
        public const string MSG_ACCOUNT_INACTIVATED = "Conta inativada com sucesso";
        public const string MSG_BALANCE_RETRIEVED = "Saldo consultado com sucesso";
        public const string MSG_ACCOUNTS_FOUND = "Contas encontradas com sucesso";
        public const string MSG_NO_ACCOUNTS_FOUND = "Nenhuma conta encontrada";
        
        // Valores de teste
        public const string TEST_CPF_VALID = "950.751.920-33";
        public const string TEST_CPF_INVALID = "123.456.789-00";
        public const string TEST_ACCOUNT_ID = "95075192033";
        public const string TEST_ACCOUNT_ID_2 = "98765432109";
        public const string TEST_INVALID_ACCOUNT_ID = "11111111111";
        public const int TEST_ACCOUNT_NUMBER = 12345;
        public const int TEST_ACCOUNT_NUMBER_2 = 54321;
        public const string TEST_NAME = "João Silva";
        public const string TEST_NAME_2 = "Maria Silva";
        public const string TEST_PASSWORD = "senha123";
        public const decimal TEST_VALID_VALUE = 100;
        public const decimal TEST_INVALID_VALUE = -100;
        public const char TEST_CREDIT_TYPE = 'C';
        public const char TEST_DEBIT_TYPE = 'D';
        public const char TEST_INVALID_TYPE = 'X';
    }
}
