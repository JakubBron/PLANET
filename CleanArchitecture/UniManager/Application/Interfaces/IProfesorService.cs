using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Interfaces
{
    public interface IProfesorService
    {
        Task<Profesor> CreateProfesorAsync(string imie, string nazwisko, string tytulNaukowy, Adres adres, int wydzialId, int? gabinetId, string prefix="P");
        Task<List<Profesor>> GetAllProfesorzyAsync();
        Task<Profesor?> GetProfesorByIdAsync(int id);
        Task<Profesor?> UpdateProfesorAsync(int id, string? imie = null, string? nazwisko = null, string? tytulNaukowy = null, Adres? adres = null, int? wydzialId = null, int? gabinetId = null);
        Task DeleteProfesorAsync(int id);
    }
}