using APIContaCorrente.Domain.Entities;

namespace APIContaCorrente.Domain.Repositories
{
    public interface IMovimentoRepository
    {
        Task<Movimento?> GetByIdAsync(string id);
        Task<IEnumerable<Movimento>> GetByContaCorrenteAsync(string idContaCorrente);
        Task<Movimento> AddAsync(Movimento movimento);
        Task<Movimento> UpdateAsync(Movimento movimento);
        Task<bool> DeleteAsync(string id);
        Task<decimal> GetSaldoAsync(string idContaCorrente);
    }
}
