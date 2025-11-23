using Domain.Entities;

namespace Application.Interfaces;

public interface IGabinetService
{
    Task<Gabinet> CreateGabinetAsync(string numerGabinetu, int? profesorId = null);
    Task<List<Gabinet>> GetAllGabinetyAsync();
    Task<Gabinet?> GetGabinetByIdAsync(int id);
    Task<Gabinet?> UpdateGabinetAsync(int id, string? numerGabinetu = null, int? profesorId = null);
    Task DeleteGabinetAsync(int id);
}