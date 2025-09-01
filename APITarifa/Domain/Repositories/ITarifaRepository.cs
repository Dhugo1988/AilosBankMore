using APITarifa.Domain.Entities;

namespace APITarifa.Domain.Repositories
{
    public interface ITarifaRepository
    {
        Task<Tarifa?> GetByIdAsync(string idTarifa);
        Task<List<Tarifa>> GetByContaCorrenteAsync(string idContaCorrente);
        Task AddAsync(Tarifa tarifa);
        Task UpdateAsync(Tarifa tarifa);
        Task DeleteAsync(string idTarifa);
        Task<bool> ExistsAsync(string idTarifa);
    }
}
