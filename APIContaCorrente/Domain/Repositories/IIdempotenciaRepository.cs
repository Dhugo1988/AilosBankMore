using APIContaCorrente.Domain.Entities;

namespace APIContaCorrente.Domain.Repositories
{
    public interface IIdempotenciaRepository
    {
        Task<Idempotencia?> GetByChaveAsync(string chave);
        Task<Idempotencia> AddAsync(Idempotencia idempotencia);
        Task<bool> ExistsAsync(string chave);
    }
}
