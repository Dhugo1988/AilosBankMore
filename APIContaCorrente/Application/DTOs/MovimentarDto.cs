using System.ComponentModel.DataAnnotations;

namespace APIContaCorrente.Application.DTOs
{
    public class MovimentarDto
    {
        [Required(ErrorMessage = "Identificador da requisição é obrigatório")]
        [StringLength(37, ErrorMessage = "Identificador da requisição deve ter no máximo 37 caracteres")]
        public string IdRequisicao { get; set; } = string.Empty;
        [Required(ErrorMessage = "Valor é obrigatório")]
        public decimal Valor { get; set; }
        [Required(ErrorMessage = "Tipo de movimento é obrigatório")]
        public char TipoMovimento { get; set; }
        public int? NumeroConta { get; set; }
    }
}
