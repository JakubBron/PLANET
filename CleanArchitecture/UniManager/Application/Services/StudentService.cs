using Application.Interfaces;
using Domain.Entities;
using Domain.ValueObjects;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class StudentService : IStudentService
    {
        private readonly UniversityDbContext _context;
        private readonly ILicznikIndeksowService _licznikIndeksowService;

        public StudentService(UniversityDbContext context, ILicznikIndeksowService licznikIndeksowService)
        {
            _context = context;
            _licznikIndeksowService = licznikIndeksowService;
        }

        // CREATE
        public async Task<Student> CreateStudentAsync(string imie, string nazwisko, int rokStudiow, Adres adres, string prefix="S")
        {
            //using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                int numer = await _licznikIndeksowService.GetNextAsync(prefix);
                string nowyIndeks = $"{prefix}{numer}";

                var student = new Student
                {
                    Imie = imie,
                    Nazwisko = nazwisko,
                    RokStudiow = rokStudiow,
                    AdresZamieszkania = adres,
                    IndeksUczelniany = nowyIndeks
                };

                _context.Studenci.Add(student);
                await _context.SaveChangesAsync();
                //await transaction.CommitAsync();

                return student;
            }
            catch
            {
                //await transaction.RollbackAsync();
                throw;
            }
        }

        // READ ALL
        public async Task<List<Student>> GetAllStudentsAsync()
        {
            return await _context.Studenci
                .Include(s => s.AdresZamieszkania)
                .ToListAsync();
        }

        // READ BY ID
        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            return await _context.Studenci
                .Include(s => s.AdresZamieszkania)
                .SingleOrDefaultAsync(s => s.Id == id);
        }

        // UPDATE
        public async Task<Student?> UpdateStudentAsync(int id, string? imie = null, string? nazwisko = null, int? rokStudiow = null, Adres? adres = null)
        {
            var student = await _context.Studenci.FindAsync(id);
            if (student == null) return null;

            if (!string.IsNullOrWhiteSpace(imie)) student.Imie = imie;
            if (!string.IsNullOrWhiteSpace(nazwisko)) student.Nazwisko = nazwisko;
            if (rokStudiow.HasValue) student.RokStudiow = rokStudiow.Value;
            if (adres != null) student.AdresZamieszkania = adres;

            await _context.SaveChangesAsync();
            return student;
        }

        // DELETE
        public async Task DeleteStudentAsync(int id)
        {
            var student = await _context.Studenci.FindAsync(id);
            if (student == null) return;

            var numer = student.IndeksUczelniany;
            if (numer != null)
            {
                _licznikIndeksowService.DecrementIfLast(numer);
            }

            _context.Studenci.Remove(student);
            await _context.SaveChangesAsync();
        }
    }
}
