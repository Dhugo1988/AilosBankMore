using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APITarifa.Domain.Entities
{
    public class Tarifa
    {
        [Key]
        [Column("idtarifa")]
        public string IdTarifa { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [Column("idcontacorrente")]
        public string IdContaCorrente { get; set; } = string.Empty; 

        [Required]
        [Column("datamovimento")]
        public string DataMovimento { get; set; } = string.Empty; 

        [Required]
        [Column("valor")]
        public decimal Valor { get; set; } 
    }
}
