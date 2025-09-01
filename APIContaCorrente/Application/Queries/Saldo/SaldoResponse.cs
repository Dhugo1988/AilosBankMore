using APIContaCorrente.Application.Common.Model;

namespace APIContaCorrente.Application.Queries.Saldo
{
    public class SaldoResponse : BaseResponse
    {
        public int? NumeroConta { get; set; }
        public string? NomeTitular { get; set; }
        public DateTime? Timestamp { get; set; }
        public decimal? Saldo { get; set; }

        public SaldoResponse(bool success, string? message = null, string? errorType = null)
            : base(success, message, errorType) 
        { 
        }

        public SaldoResponse(bool success, int? numeroConta = null, string? nomeTitular = null, 
            DateTime? timestamp = null, decimal? saldo = null) : base(success)
        {
            NumeroConta = numeroConta;
            NomeTitular = nomeTitular;
            Timestamp = timestamp;
            Saldo = saldo;
        }
    }
}
