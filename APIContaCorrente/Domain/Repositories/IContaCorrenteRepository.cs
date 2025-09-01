using APIContaCorrente.Domain.Entities;

namespace APIContaCorrente.Domain.Repositories
{
    public interface IContaCorrenteRepository
    {
        Task<ContaCorrente?> GetByIdAsync(string id);
        Task<ContaCorrente?> GetByNumeroAsync(int numero);
        Task<ContaCorrente?> GetByCpfAsync(string cpf);
        Task<IEnumerable<ContaCorrente>> GetAllAsync();
        Task<ContaCorrente> AddAsync(ContaCorrente contaCorrente);
        Task<ContaCorrente> UpdateAsync(ContaCorrente contaCorrente);
        Task<bool> DeleteAsync(string id);
        Task<bool> ExistsAsync(string id);
        Task<bool> ExistsByNumeroAsync(int numero);
        Task<bool> ExistsByCpfAsync(string cpf);
    }
}
