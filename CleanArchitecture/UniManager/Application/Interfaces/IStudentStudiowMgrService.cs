using Domain.Entities;
using Domain.ValueObjects;


namespace Application.Interfaces
{
    public interface IStudentStudiowMgrService
    {
        Task<StudentStudiowMgr> CreateStudentMgrAsync(string imie, string nazwisko, int rokStudiow, Adres adres, string tematPracy, int promotorId, string prefix = "S");
        Task<List<StudentStudiowMgr>> GetAllStudentsMgrAsync();
        Task<StudentStudiowMgr?> GetStudentMgrByIdAsync(int id);
        Task<StudentStudiowMgr?> UpdateStudentMgrAsync(int id, string? imie = null, string? nazwisko = null, int? rokStudiow = null, Adres? adres = null, string? tematPracy = null, int? promotorId = null);
        Task DeleteStudentMgrAsync(int id);
    }
}