using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APITarifa.Domain.Entities
{
    public class Idempotencia
    {
        [Key]
        [StringLength(37)]
        [Column("chave_idempotencia")]
        public string ChaveIdempotencia { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [StringLength(1000)]
        [Column("requisicao")]
        public string? Requisicao { get; set; }

        [Required]
        [StringLength(1000)]
        [Column("resultado")]
        public string? Resultado { get; set; }

    }
}
