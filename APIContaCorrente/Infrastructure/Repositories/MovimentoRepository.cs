using APIContaCorrente.Application.Common.Constants;
using APIContaCorrente.Domain.Entities;
using APIContaCorrente.Domain.Repositories;
using APIContaCorrente.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace APIContaCorrente.Infrastructure.Repositories
{
    public class MovimentoRepository : IMovimentoRepository
    {
        private readonly ContaCorrenteDbContext _context;
        private readonly ILogger<MovimentoRepository> _logger;

        public MovimentoRepository(ContaCorrenteDbContext context, ILogger<MovimentoRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Movimento?> GetByIdAsync(string id)
        {
            return await _context.Movimentos
                .Include(m => m.ContaCorrente)
                .FirstOrDefaultAsync(m => m.IdMovimento == id);
        }

        public async Task<IEnumerable<Movimento>> GetByContaCorrenteAsync(string idContaCorrente)
        {
            return await _context.Movimentos
                .Include(m => m.ContaCorrente)
                .Where(m => m.IdContaCorrente == idContaCorrente)
                .OrderBy(m => m.DataMovimento)
                .ToListAsync();
        }

        public async Task<Movimento> AddAsync(Movimento movimento)
        {
            _context.Movimentos.Add(movimento);
            await _context.SaveChangesAsync();
            return movimento;
        }

        public async Task<Movimento> UpdateAsync(Movimento movimento)
        {
            _context.Movimentos.Update(movimento);
            await _context.SaveChangesAsync();
            return movimento;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var movimento = await GetByIdAsync(id);
            if (movimento == null) return false;

            _context.Movimentos.Remove(movimento);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<decimal> GetSaldoAsync(string idContaCorrente)
        {
            var movimentos = await GetByContaCorrenteAsync(idContaCorrente);
            
            var creditos = movimentos.Where(m => m.TipoMovimento == ValidationConstants.TIPO_CREDITO).Sum(m => m.Valor);
            var debitos = movimentos.Where(m => m.TipoMovimento == ValidationConstants.TIPO_DEBITO).Sum(m => m.Valor);
            _logger.LogInformation("[Saldo] Conta={Conta} Creditos={Creditos} Debitos={Debitos} Movs={Count}", idContaCorrente, creditos, debitos, movimentos.Count());
            foreach (var m in movimentos)
            {
                _logger.LogInformation("[Saldo] Item Conta={Conta} Tipo={Tipo} Valor={Valor}", m.IdContaCorrente, m.TipoMovimento, m.Valor);
            }
            
            return creditos - debitos;
        }
    }
}
