using System.ComponentModel.DataAnnotations;

namespace APITransferencia.Application.DTOs
{
    public class EfetuarTransferenciaDto
    {
        [Required(ErrorMessage = "ID da requisição é obrigatório")]
        [StringLength(37, ErrorMessage = "ID da requisição deve ter no máximo 37 caracteres")]
        public string IdRequisicao { get; set; } = string.Empty;

        [Required(ErrorMessage = "Número da conta de destino é obrigatório")]
        public int NumeroContaDestino { get; set; }

        [Required(ErrorMessage = "Valor é obrigatório")]
        public decimal Valor { get; set; }
    }
}
