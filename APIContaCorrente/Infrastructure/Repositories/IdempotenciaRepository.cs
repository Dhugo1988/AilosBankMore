using Microsoft.EntityFrameworkCore;
using APIContaCorrente.Domain.Entities;
using APIContaCorrente.Domain.Repositories;
using APIContaCorrente.Infrastructure.Data;

namespace APIContaCorrente.Infrastructure.Repositories
{
    public class IdempotenciaRepository : IIdempotenciaRepository
    {
        private readonly ContaCorrenteDbContext _context;

        public IdempotenciaRepository(ContaCorrenteDbContext context)
        {
            _context = context;
        }

        public async Task<Idempotencia?> GetByChaveAsync(string chave)
        {
            return await _context.Idempotencias
                .FirstOrDefaultAsync(i => i.ChaveIdempotencia == chave);
        }

        public async Task<Idempotencia> AddAsync(Idempotencia idempotencia)
        {
            _context.Idempotencias.Add(idempotencia);
            await _context.SaveChangesAsync();
            return idempotencia;
        }

        public async Task<bool> ExistsAsync(string chave)
        {
            return await _context.Idempotencias.AnyAsync(i => i.ChaveIdempotencia == chave);
        }
    }
}
