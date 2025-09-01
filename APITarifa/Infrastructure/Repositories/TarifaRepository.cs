using APITarifa.Domain.Entities;
using APITarifa.Domain.Repositories;
using APITarifa.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace APITarifa.Infrastructure.Repositories
{
    public class TarifaRepository : ITarifaRepository
    {
        private readonly TarifaDbContext _context;

        public TarifaRepository(TarifaDbContext context)
        {
            _context = context;
        }

        public async Task<Tarifa?> GetByIdAsync(string idTarifa)
        {
            return await _context.Tarifas.FindAsync(idTarifa);
        }

        public async Task<List<Tarifa>> GetByContaCorrenteAsync(string idContaCorrente)
        {
            return await _context.Tarifas
                .Where(t => t.IdContaCorrente == idContaCorrente)
                .OrderByDescending(t => t.DataMovimento)
                .ToListAsync();
        }

        public async Task AddAsync(Tarifa tarifa)
        {
            await _context.Tarifas.AddAsync(tarifa);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Tarifa tarifa)
        {
            _context.Tarifas.Update(tarifa);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string idTarifa)
        {
            var tarifa = await GetByIdAsync(idTarifa);
            if (tarifa != null)
            {
                _context.Tarifas.Remove(tarifa);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(string idTarifa)
        {
            return await _context.Tarifas.AnyAsync(t => t.IdTarifa == idTarifa);
        }
    }
}
