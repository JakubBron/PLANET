using Application.Interfaces;
using Domain.Entities;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class KursService : IKursService
    {
        private readonly UniversityDbContext _context;

        public KursService(UniversityDbContext context)
        {
            _context = context;
        }

        // CREATE
        public async Task<Kurs> CreateKursAsync(string nazwa, string kod, int ects, int prowadzacyId, int wydzialId, List<int>? prerequisiteIds = null)
        {
            var prowadzacy = await _context.Profesorzy.FindAsync(prowadzacyId);
            var wydzial = await _context.Wydzialy.FindAsync(wydzialId);

            if (prowadzacy == null) throw new InvalidOperationException("Nie znaleziono profesora");
            if (wydzial == null) throw new InvalidOperationException("Nie znaleziono wydziału");

            var kurs = new Kurs
            {
                Nazwa = nazwa,
                Kod = kod,
                ECTS = ects,
                Prowadzacy = prowadzacy,
                Wydzial = wydzial,
                ProfesorId = prowadzacyId,
                WydzialId = wydzialId
            };

            // obsługa prerekwizytów
            if (prerequisiteIds != null)
            {
                foreach (var preId in prerequisiteIds)
                {
                    var pre = await _context.Kursy.FindAsync(preId);
                    if (pre != null)
                    {
                        kurs.Prerequisites.Add(pre);       // kurs wymaga pre
                        pre.IsPrerequisiteFor.Add(kurs);   // pre jest wymagany dla kursu
                    }
                    else throw new InvalidOperationException($"Nie znaleziono kursu o Id={preId} jako prerekwizytu.");
                }
            }

            _context.Kursy.Add(kurs);
            await _context.SaveChangesAsync();
            return kurs;
        }

        // READ ALL
        public async Task<List<Kurs>> GetAllKursyAsync()
        {
            return await _context.Kursy
                .Include(k => k.Prowadzacy)
                .Include(k => k.Wydzial)
                .Include(k => k.Prerequisites)
                .Include(k => k.IsPrerequisiteFor)
                .ToListAsync();
        }

        // READ BY ID
        public async Task<Kurs?> GetKursByIdAsync(int id)
        {
            return await _context.Kursy
                .Include(k => k.Prowadzacy)
                .Include(k => k.Wydzial)
                .Include(k => k.Prerequisites)
                .Include(k => k.IsPrerequisiteFor)
                .SingleOrDefaultAsync(k => k.Id == id);
        }

        // UPDATE
        public async Task<Kurs?> UpdateKursAsync(int id, string? nazwa = null, string? kod = null, int? ects = null, int? prowadzacyId = null, int? wydzialId = null, List<int>? prerequisiteIds = null)
        {
            var kurs = await _context.Kursy
                .Include(k => k.Prerequisites)
                .Include(k => k.IsPrerequisiteFor)
                .SingleOrDefaultAsync(k => k.Id == id);

            if (kurs == null) return null;

            if (!string.IsNullOrWhiteSpace(nazwa)) kurs.Nazwa = nazwa;
            if (!string.IsNullOrWhiteSpace(kod)) kurs.Kod = kod;
            if (ects.HasValue) kurs.ECTS = ects.Value;

            // needed, because I can assign course to another professor or faculty
            if (prowadzacyId.HasValue)
            {
                var prowadzacy = await _context.Profesorzy.FindAsync(prowadzacyId.Value);
                if (prowadzacy != null)
                {
                    kurs.Prowadzacy = prowadzacy;
                    kurs.ProfesorId = prowadzacyId.Value;
                }
            }

            // needed, because I can assign course to another professor or faculty
            if (wydzialId.HasValue)
            {
                var wydzial = await _context.Wydzialy.FindAsync(wydzialId.Value);
                if (wydzial != null)
                {
                    kurs.Wydzial = wydzial;
                    kurs.WydzialId = wydzialId.Value;
                }
            }

            if (prerequisiteIds != null)
            {
                // Usuń stare relacje dwukierunkowe
                foreach (var oldPre in kurs.Prerequisites.ToList())
                {
                    oldPre.IsPrerequisiteFor.Remove(kurs);
                }
                kurs.Prerequisites.Clear();

                // Dodaj nowe relacje dwukierunkowe
                foreach (var preId in prerequisiteIds)
                {
                    var pre = await _context.Kursy.FindAsync(preId);
                    if (pre != null)
                    {
                        kurs.Prerequisites.Add(pre);
                        pre.IsPrerequisiteFor.Add(kurs);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return kurs;
        }

        // DELETE
        public async Task DeleteKursAsync(int id)
        {
            var kurs = await _context.Kursy
                .Include(k => k.Prerequisites)
                .Include(k => k.IsPrerequisiteFor)
                .SingleOrDefaultAsync(k => k.Id == id);

            if (kurs == null) return;

            // Usuń kurs z list IsPrerequisiteFor innych kursów
            foreach (var pre in kurs.Prerequisites.ToList())
            {
                pre.IsPrerequisiteFor.Remove(kurs);
            }

            // Usuń kurs z list Prerequisites kursów, dla których był wymaganiem
            foreach (var dep in kurs.IsPrerequisiteFor.ToList())
            {
                dep.Prerequisites.Remove(kurs);
            }

            _context.Kursy.Remove(kurs);
            await _context.SaveChangesAsync();
        }
        // Add a prerequisite to a course
        public async Task<Kurs?> AddPrerequisiteAsync(int kursId, int prerequisiteId)
        {
            if (kursId == prerequisiteId)
                throw new InvalidOperationException("A course cannot be a prerequisite of itself.");

            var kurs = await _context.Kursy
                .Include(k => k.Prerequisites)
                .Include(k => k.IsPrerequisiteFor)
                .SingleOrDefaultAsync(k => k.Id == kursId);

            var prerequisite = await _context.Kursy
                .Include(k => k.Prerequisites)
                .Include(k => k.IsPrerequisiteFor)
                .SingleOrDefaultAsync(k => k.Id == prerequisiteId);

            if (kurs == null || prerequisite == null)
                throw new InvalidOperationException("Course or prerequisite not found.");

            if (!kurs.Prerequisites.Contains(prerequisite))
            {
                kurs.Prerequisites.Add(prerequisite);
                prerequisite.IsPrerequisiteFor.Add(kurs);
                await _context.SaveChangesAsync();
            }

            return kurs;
        }

        // Remove a prerequisite from a course
        public async Task<Kurs?> RemovePrerequisiteAsync(int kursId, int prerequisiteId)
        {
            var kurs = await _context.Kursy
                .Include(k => k.Prerequisites)
                .Include(k => k.IsPrerequisiteFor)
                .SingleOrDefaultAsync(k => k.Id == kursId);

            var prerequisite = await _context.Kursy
                .Include(k => k.Prerequisites)
                .Include(k => k.IsPrerequisiteFor)
                .SingleOrDefaultAsync(k => k.Id == prerequisiteId);

            if (kurs == null)
                throw new InvalidOperationException("Nie znaleziono kursu.");
            
            if (prerequisite == null)
                throw new InvalidOperationException("Nie znaleziono prerekwizytu.");

            if (kurs.Prerequisites.Contains(prerequisite))
            {
                kurs.Prerequisites.Remove(prerequisite);
                prerequisite.IsPrerequisiteFor.Remove(kurs);
                await _context.SaveChangesAsync();
            }

            return kurs;
        }

    }
}
