using Domain.Entities;
using Domain.ValueObjects;

namespace Application.Interfaces
{
    public interface IProfesorService
    {
        // CREATE
        Task<Profesor> CreateProfesorAsync(string imie, string nazwisko, string tytulNaukowy, Adres adres, int wydzialId, int? gabinetId, string prefix="P");

        // READ ALL
        Task<List<Profesor>> GetAllProfesorzyAsync();

        // READ BY ID
        Task<Profesor?> GetProfesorByIdAsync(int id);

        // UPDATE
        Task<Profesor?> UpdateProfesorAsync(int id, string? imie = null, string? nazwisko = null, string? tytulNaukowy = null, Adres? adres = null, int? wydzialId = null, int? gabinetId = null);

        // DELETE
        Task DeleteProfesorAsync(int id);
    }
}