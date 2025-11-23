using Domain.Entities;

namespace Application.Interfaces;

public interface IKursService
{
    Task<Kurs> CreateKursAsync(string nazwa, string kod, int ects, int prowadzacyId, int wydzialId, List<int>? prerequisiteIds = null);
    Task<List<Kurs>> GetAllKursyAsync();
    Task<Kurs?> GetKursByIdAsync(int id);
    Task<Kurs?> UpdateKursAsync(int id, string? nazwa = null, string? kod = null, int? ects = null, int? prowadzacyId = null, int? wydzialId = null, List<int>? prerequisiteIds = null);
    Task DeleteKursAsync(int id);
    Task<Kurs?> AddPrerequisiteAsync(int kursId, int prerequisiteId);
    Task<Kurs?> RemovePrerequisiteAsync(int kursId, int prerequisiteId);
}