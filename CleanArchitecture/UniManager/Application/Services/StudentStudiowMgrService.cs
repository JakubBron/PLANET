using Application.Interfaces;
using Domain.Entities;
using Domain.ValueObjects;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class StudentStudiowMgrService : IStudentStudiowMgrService
    {
        private readonly UniversityDbContext _context;
        private readonly ILicznikIndeksowService _licznikIndeksowService;

        public StudentStudiowMgrService(UniversityDbContext context, ILicznikIndeksowService licznikIndeksowService)
        {
            _context = context;
            _licznikIndeksowService = licznikIndeksowService;
        }

        public async Task<StudentStudiowMgr> CreateStudentMgrAsync(string imie, string nazwisko, int rokStudiow, Adres adres, string tematPracy, int promotorId, string prefix="S")
        {
            try
            {
                var promotor = await _context.Profesorzy.FindAsync(promotorId);
                if (promotor == null)
                    throw new InvalidOperationException("Promotor nie istnieje.");

                int numer = await _licznikIndeksowService.GetNextAsync(prefix);
                string nowyIndeks = $"{prefix}{numer}";

                var student = new StudentStudiowMgr()
                {
                    Imie = imie,
                    Nazwisko = nazwisko,
                    RokStudiow = rokStudiow,
                    AdresZamieszkania = adres,
                    IndeksUczelniany = nowyIndeks,
                    TematPracyDyplomowej = tematPracy,
                    PromotorId = promotorId,
                    Promotor = promotor
                };

                _context.Studenci.Add(student);
                await _context.SaveChangesAsync();
                return student;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public async Task<List<StudentStudiowMgr>> GetAllStudentsMgrAsync()
        {
            return await _context.Studenci
                .OfType<StudentStudiowMgr>()
                .Include(s => s.AdresZamieszkania)
                .Include(s => s.Promotor)
                .ToListAsync();
        }

        public async Task<StudentStudiowMgr?> GetStudentMgrByIdAsync(int id)
        {
            return await _context.Studenci
                .OfType<StudentStudiowMgr>()
                .Include(s => s.AdresZamieszkania)
                .Include(s => s.Promotor)
                .SingleOrDefaultAsync(s => s.Id == id);
        }

        public async Task<StudentStudiowMgr?> UpdateStudentMgrAsync(int id, string? imie = null, string? nazwisko = null, int? rokStudiow = null, Adres? adres = null, string? tematPracy = null, int? promotorId = null)
        {
            var student = await _context.Studenci.OfType<StudentStudiowMgr>().SingleOrDefaultAsync(s => s.Id == id);
            if (student == null) return null;

            if (!string.IsNullOrWhiteSpace(imie)) student.Imie = imie;
            if (!string.IsNullOrWhiteSpace(nazwisko)) student.Nazwisko = nazwisko;
            if (rokStudiow.HasValue) student.RokStudiow = rokStudiow.Value;
            if (adres != null) student.AdresZamieszkania = adres;
            if (!string.IsNullOrWhiteSpace(tematPracy)) student.TematPracyDyplomowej = tematPracy;
            if (promotorId.HasValue)
            {
                var promotor = await _context.Profesorzy.FindAsync(promotorId.Value);
                if (promotor == null)
                    throw new InvalidOperationException("Promotor nie istnieje.");
                student.Promotor = promotor;
                student.PromotorId = promotorId;
            }

            await _context.SaveChangesAsync();
            return student;
        }

        public async Task DeleteStudentMgrAsync(int id)
        {
            var student = await _context.Studenci.OfType<StudentStudiowMgr>().SingleOrDefaultAsync(s => s.Id == id);
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
