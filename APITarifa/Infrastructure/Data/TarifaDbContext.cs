using APITarifa.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace APITarifa.Infrastructure.Data
{
    public class TarifaDbContext : DbContext
    {
        public TarifaDbContext(DbContextOptions<TarifaDbContext> options) : base(options)
        {
        }

        public DbSet<Tarifa> Tarifas { get; set; }
        public DbSet<Idempotencia> Idempotencias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Tarifa>(entity =>
            {
                entity.ToTable("tarifa");
                entity.HasKey(e => e.IdTarifa);
                entity.Property(e => e.IdTarifa).HasColumnName("idtarifa").HasMaxLength(37).IsRequired();
                entity.Property(e => e.IdContaCorrente).HasColumnName("idcontacorrente").HasMaxLength(37).IsRequired();
                entity.Property(e => e.DataMovimento).HasColumnName("datamovimento").HasMaxLength(25).IsRequired();
                entity.Property(e => e.Valor).HasColumnName("valor").HasColumnType("REAL").IsRequired();
                
                entity.HasIndex(e => e.IdContaCorrente);
            });

            modelBuilder.Entity<Idempotencia>(entity =>
            {
                entity.ToTable("idempotencia");
                entity.HasKey(e => e.ChaveIdempotencia);
                entity.Property(e => e.ChaveIdempotencia).HasColumnName("chave_idempotencia").HasMaxLength(255).IsRequired();
                entity.Property(e => e.Requisicao).HasColumnName("requisicao").IsRequired();
                entity.Property(e => e.Resultado).HasColumnName("resultado").IsRequired();
            });
        }
    }
}
