namespace APITransferencia.Application.DTOs
{
    public class ContaCorrenteResponse
    {
        public string IdContaCorrente { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public int Numero { get; set; }
        public bool Ativo { get; set; }
        public decimal Saldo { get; set; }
        public string? ErrorType { get; set; }
        public string? Message { get; set; }
    }
}

