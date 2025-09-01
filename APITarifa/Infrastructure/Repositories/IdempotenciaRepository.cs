using APITarifa.Domain.Entities;
using APITarifa.Domain.Repositories;
using APITarifa.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace APITarifa.Infrastructure.Repositories
{
    public class IdempotenciaRepository : IIdempotenciaRepository
    {
        private readonly TarifaDbContext _context;

        public IdempotenciaRepository(TarifaDbContext context)
        {
            _context = context;
        }

        public async Task<Idempotencia?> GetByChaveAsync(string chaveIdempotencia)
        {
            return await _context.Idempotencias.FindAsync(chaveIdempotencia);
        }

        public async Task AddAsync(Idempotencia idempotencia)
        {
            await _context.Idempotencias.AddAsync(idempotencia);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string chaveIdempotencia)
        {
            return await _context.Idempotencias.AnyAsync(i => i.ChaveIdempotencia == chaveIdempotencia);
        }
    }
}
