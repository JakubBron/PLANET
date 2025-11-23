using Domain.Entities;

namespace Application.Interfaces
{
    public interface IWydzialService
    {
        Task<Wydzial> CreateWydzialAsync(string nazwa);
        Task<List<Wydzial>> GetAllWydzialyAsync();
        Task<Wydzial?> GetWydzialByIdAsync(int id);
        Task<Wydzial?> UpdateWydzialAsync(int id, string? nazwa = null);
        Task DeleteWydzialAsync(int id);
    }
}