using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIContaCorrente.Domain.Entities
{
    public class ContaCorrente
    {
        [Key]
        [MaxLength(37)]
        [Column("idcontacorrente")]
        public string IdContaCorrente { get; set; } = string.Empty;

        [Required]
        [Column("numero")]
        public int Numero { get; set; }
        
        [Required]
        [MaxLength(100)]
        [Column("nome")]
        public string Nome { get; set; } = string.Empty;
        
        [Required]
        [Column("ativo")]
        public bool Ativo { get; set; } = true;
        
        [Required]
        [MaxLength(100)]
        [Column("senha")]
        public string Senha { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(100)]
        [Column("salt")]
        public string Salt { get; set; } = string.Empty;
        
        public virtual ICollection<Movimento> Movimentos { get; set; } = new List<Movimento>();

        public void Desativar()
        {
            Ativo = false;
        }

        public void Reativar()
        {
            Ativo = true;
        }
    }
}
