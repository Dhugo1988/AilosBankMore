using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APITransferencia.Domain.Entities
{
    public class Transferencia
    {
        [Required]
        [StringLength(37)]
        [Column("idtransferencia")]
        public string IdTransferencia { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [StringLength(37)]
        [Column("idcontacorrente_origem")]
        public string IdContaCorrenteOrigem { get; set; } = string.Empty;

        [Required]
        [StringLength(37)]
        [Column("idcontacorrente_destino")]
        public string IdContaCorrenteDestino { get; set; } = string.Empty;

        [Required]
        [StringLength(25)]
        [Column("datamovimento")]
        public string DataMovimento { get; set; } = string.Empty;

        [Required]
        [Column("valor")]
        public decimal Valor { get; set; }

        // Construtor sem parâmetros para o Entity Framework
        public Transferencia() { }

        // Construtor para criação de novas transferências
        public Transferencia(string origem, string destino, string data, decimal valor)
        {
            IdContaCorrenteOrigem = origem;
            IdContaCorrenteDestino = destino;
            DataMovimento = data;
            Valor = valor;
        }
    }
}
