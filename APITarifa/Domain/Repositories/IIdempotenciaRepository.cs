using APITarifa.Domain.Entities;

namespace APITarifa.Domain.Repositories
{
    public interface IIdempotenciaRepository
    {
        Task<Idempotencia?> GetByChaveAsync(string chaveIdempotencia);
        Task AddAsync(Idempotencia idempotencia);
        Task<bool> ExistsAsync(string chaveIdempotencia);
    }
}
