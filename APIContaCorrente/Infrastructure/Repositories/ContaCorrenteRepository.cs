using Microsoft.EntityFrameworkCore;
using APIContaCorrente.Domain.Entities;
using APIContaCorrente.Domain.Repositories;
using APIContaCorrente.Infrastructure.Data;

namespace APIContaCorrente.Infrastructure.Repositories
{
    public class ContaCorrenteRepository : IContaCorrenteRepository
    {
        private readonly ContaCorrenteDbContext _context;

        public ContaCorrenteRepository(ContaCorrenteDbContext context)
        {
            _context = context;
        }

        public async Task<ContaCorrente?> GetByIdAsync(string id)
        {
            return await _context.ContasCorrentes
                .Include(c => c.Movimentos)
                .FirstOrDefaultAsync(c => c.IdContaCorrente == id);
        }

        public async Task<ContaCorrente?> GetByNumeroAsync(int numero)
        {
            return await _context.ContasCorrentes
                .Include(c => c.Movimentos)
                .FirstOrDefaultAsync(c => c.Numero == numero);
        }

        public async Task<ContaCorrente?> GetByCpfAsync(string cpf)
        {
            return await _context.ContasCorrentes
                .Include(c => c.Movimentos)
                .FirstOrDefaultAsync(c => c.IdContaCorrente == cpf);
        }

        public async Task<IEnumerable<ContaCorrente>> GetAllAsync()
        {
            return await _context.ContasCorrentes
                .Include(c => c.Movimentos)
                .ToListAsync();
        }

        public async Task<ContaCorrente> AddAsync(ContaCorrente contaCorrente)
        {
            _context.ContasCorrentes.Add(contaCorrente);
            await _context.SaveChangesAsync();
            return contaCorrente;
        }

        public async Task<ContaCorrente> UpdateAsync(ContaCorrente contaCorrente)
        {
            _context.ContasCorrentes.Update(contaCorrente);
            await _context.SaveChangesAsync();
            return contaCorrente;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var conta = await GetByIdAsync(id);
            if (conta == null) return false;

            _context.ContasCorrentes.Remove(conta);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.ContasCorrentes.AnyAsync(c => c.IdContaCorrente == id);
        }

        public async Task<bool> ExistsByNumeroAsync(int numero)
        {
            return await _context.ContasCorrentes.AnyAsync(c => c.Numero == numero);
        }

        public async Task<bool> ExistsByCpfAsync(string cpf)
        {
            return await _context.ContasCorrentes.AnyAsync(c => c.IdContaCorrente == cpf);
        }
    }
}
