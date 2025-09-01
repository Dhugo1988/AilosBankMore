using Microsoft.EntityFrameworkCore;
using APITransferencia.Domain.Entities;

namespace APITransferencia.Infrastructure.Data
{
    public interface ITransferenciaDbContext
    {
        DbSet<Transferencia> Transferencias { get; set; }
        DbSet<Idempotencia> Idempotencias { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
