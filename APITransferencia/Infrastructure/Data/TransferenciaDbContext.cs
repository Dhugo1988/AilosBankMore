using Microsoft.EntityFrameworkCore;
using APITransferencia.Domain.Entities;

namespace APITransferencia.Infrastructure.Data
{
    public class TransferenciaDbContext : DbContext, ITransferenciaDbContext
    {
        public TransferenciaDbContext(DbContextOptions<TransferenciaDbContext> options) : base(options) { }

        public DbSet<Transferencia> Transferencias { get; set; }
        public DbSet<Idempotencia> Idempotencias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Transferencia>(e =>
            {
                e.ToTable("transferencia");
                e.HasKey(x => x.IdTransferencia);
                e.Property(x => x.IdTransferencia).HasColumnName("idtransferencia").HasMaxLength(37).IsRequired();
                e.Property(x => x.IdContaCorrenteOrigem).HasColumnName("idcontacorrente_origem").HasMaxLength(37).IsRequired();
                e.Property(x => x.IdContaCorrenteDestino).HasColumnName("idcontacorrente_destino").HasMaxLength(37).IsRequired();
                e.Property(x => x.DataMovimento).HasColumnName("datamovimento").HasMaxLength(25).IsRequired();
                e.Property(x => x.Valor).HasColumnName("valor").IsRequired();
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
