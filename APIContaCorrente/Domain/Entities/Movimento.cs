using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIContaCorrente.Domain.Entities
{
    public class Movimento
    {
        [Key]
        [Column("idmovimento")]
        public string IdMovimento { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [Column("idcontacorrente")]
        public string IdContaCorrente { get; set; } = string.Empty;
        
        [Required]
        [Column("datamovimento")]
        public DateTime DataMovimento { get; set; }
        
        [Required]
        [MaxLength(1)]
        [Column("tipomovimento")]
        public char TipoMovimento { get; set; }
        
        [Required]
        [Column("valor")]
        public decimal Valor { get; set; }

        public virtual ContaCorrente ContaCorrente { get; set; } = null!;
    }
}
