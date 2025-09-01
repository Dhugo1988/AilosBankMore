using Microsoft.EntityFrameworkCore;
using APIContaCorrente.Domain.Entities;

namespace APIContaCorrente.Infrastructure.Data
{
    public class ContaCorrenteDbContext : DbContext
    {
        public ContaCorrenteDbContext(DbContextOptions<ContaCorrenteDbContext> options) : base(options)
        {
        }

        public DbSet<ContaCorrente> ContasCorrentes { get; set; }
        public DbSet<Movimento> Movimentos { get; set; }
        public DbSet<Idempotencia> Idempotencias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuração da entidade ContaCorrente
            modelBuilder.Entity<ContaCorrente>(entity =>
            {
                entity.HasKey(e => e.IdContaCorrente);
                entity.Property(e => e.IdContaCorrente).HasMaxLength(37);
                entity.Property(e => e.Numero).IsRequired();
                entity.Property(e => e.Nome).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Ativo).IsRequired();
                entity.Property(e => e.Senha).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Salt).HasMaxLength(100).IsRequired();
                
                entity.HasIndex(e => e.Numero).IsUnique();
                entity.HasIndex(e => e.Nome);
            });

            // Configuração da entidade Movimento
            modelBuilder.Entity<Movimento>(entity =>
            {
                entity.HasKey(e => e.IdMovimento);
                entity.Property(e => e.IdMovimento).HasMaxLength(37);
                entity.Property(e => e.IdContaCorrente).HasColumnName("idcontacorrente").HasMaxLength(37).IsRequired();
                entity.Property(e => e.DataMovimento).HasColumnName("datamovimento").IsRequired();
                entity.Property(e => e.TipoMovimento).HasColumnName("tipomovimento").HasMaxLength(1).IsRequired();
                entity.Property(e => e.Valor).HasColumnName("valor").HasColumnType("decimal(18,2)").IsRequired();
                
                entity.HasOne(e => e.ContaCorrente)
                      .WithMany(e => e.Movimentos)
                      .HasForeignKey(e => e.IdContaCorrente)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Idempotencia>(e =>
            {
                e.ToTable("idempotencia");
                e.HasKey(x => x.ChaveIdempotencia);
                e.Property(x => x.ChaveIdempotencia).HasColumnName("chave_idempotencia").HasMaxLength(37);
                e.Property(x => x.Requisicao).HasColumnName("requisicao").HasMaxLength(1000);
                e.Property(x => x.Resultado).HasColumnName("resultado").HasMaxLength(1000);
            });
        }
    }
}
