using Application.Interfaces;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.QueryDTOs;

namespace Application.Services
{
    public class QueryService : IQueryService
    {
        private readonly UniversityDbContext _context;

        public QueryService(UniversityDbContext context)
        {
            _context = context;
        }
        public async Task<ProfesorWynikDTO?> GetProfesorZNajwiekszaLiczbaStudentow()
        {
            return await _context.Kursy
                .AsNoTracking()
                .Select(k => new
                {
                    k.ProfesorId,
                    StudentId = k.Enrollmenty.Select(e => e.StudentId)
                })
                .GroupBy(x => x.ProfesorId)
                .Select(g => new ProfesorWynikDTO
                {
                    ProfesorId = g.Key,
                    LiczbaStudentow = g.SelectMany(x => x.StudentId).Distinct().Count()
                })
                .OrderByDescending(x => x.LiczbaStudentow)
                .FirstOrDefaultAsync();
        }

        public async Task<List<GpaKursDTO>> GetGpaDlaWydzialu(int wydzialId)
        {
            return await _context.Kursy
                .AsNoTracking()
                .Where(k => k.WydzialId == wydzialId)
                .Select(k => new GpaKursDTO
                {
                    KursId = k.Id,
                    Nazwa = k.Nazwa,
                    Kod = k.Kod,
                    SredniaOcen = k.Enrollmenty
                        .Where(e => e.Ocena != null)
                        .Average(e => (double?)e.Ocena),
                    LiczbaOcenionych = k.Enrollmenty
                        .Count(e => e.Ocena != null)
                })
                .ToListAsync();
        }

        public async Task<TrudnoscStudentaDTO?> GetNajtrudniejszyPlan()
        {
            return await _context.Studenci
                .AsNoTracking()
                .Select(s => new TrudnoscStudentaDTO
                {
                    StudentId = s.Id,
                    Imie = s.Imie,
                    Nazwisko = s.Nazwisko,

                    // Suma ECTS kursów
                    EctsKursow = s.Enrollmenty
                        .Select(e => e.Kurs.ECTS)
                        .Sum(),

                    // Suma DISTINCT ECTS prerekwizytów
                    EctsPrerekwizytow = s.Enrollmenty
                        .SelectMany(e => e.Kurs.Prerequisites)
                        .Select(p => p.ECTS)
                        .Distinct()
                        .Sum()
                })
                .OrderByDescending(x => x.EctsKursow + x.EctsPrerekwizytow)
                .FirstOrDefaultAsync();
        }
    }
}
