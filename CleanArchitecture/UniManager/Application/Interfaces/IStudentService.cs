using Domain.Entities;
using Domain.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IStudentService
    {
        // CREATE
        Task<Student> CreateStudentAsync(string imie, string nazwisko, int rokStudiow, Adres adres, string prefix = "S");
        // READ
        Task<List<Student>> GetAllStudentsAsync();
        Task<Student?> GetStudentByIdAsync(int id);
        // UPDATE
        Task<Student?> UpdateStudentAsync(int id, string? imie = null, string? nazwisko = null, int? rokStudiow = null, Adres? adres = null);
        // DELETE
        Task DeleteStudentAsync(int id);
    }
}