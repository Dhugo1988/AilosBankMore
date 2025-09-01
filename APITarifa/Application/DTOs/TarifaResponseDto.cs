namespace APITarifa.Application.DTOs
{
    public class TarifaResponseDto
    {
        public string IdContaCorrente { get; set; } = string.Empty;
        public int TotalTarifas { get; set; }
        public decimal ValorTotalTarifado { get; set; }
        public List<TarifaItemDto> Tarifas { get; set; } = new List<TarifaItemDto>();
    }

    public class TarifaItemDto
    {
        public string IdTarifa { get; set; } = string.Empty;
        public DateTime DataMovimento { get; set; }
        public decimal Valor { get; set; }
    }
}
