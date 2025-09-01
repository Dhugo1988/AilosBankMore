
namespace APITransferencia.Application.DTOs
{
    public class MovimentarRequestDto
    {
        public string IdRequisicao { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public char TipoMovimento { get; set; }
        public int? NumeroConta { get; set; }
    }
}
