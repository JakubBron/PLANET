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

            if (prerequisiteIds != null)
            {
                foreach (var preId in prerequisiteIds)
                {
                    var pre = await _context.Kursy.FindAsync(preId);
                    if (pre != null)
                    {
                        kurs.Prerequisites.Add(pre);
                        pre.IsPrerequisiteFor.Add(kurs);
                    }
                    else throw new InvalidOperationException($"Nie znaleziono kursu o Id={preId} jako prerekwizytu.");
                }
            }

            _context.Kursy.Add(kurs);
            await _context.SaveChangesAsync();
            return kurs;
        }

        public async Task<List<Kurs>> GetAllKursyAsync()
        {
            return await _context.Kursy
                .Include(k => k.Prowadzacy)
                .Include(k => k.Wydzial)
                .Include(k => k.Prerequisites)
                .Include(k => k.IsPrerequisiteFor)
                .ToListAsync();
        }

        public async Task<Kurs?> GetKursByIdAsync(int id)
        {
            return await _context.Kursy
                .Include(k => k.Prowadzacy)
                .Include(k => k.Wydzial)
                .Include(k => k.Prerequisites)
                .Include(k => k.IsPrerequisiteFor)
                .SingleOrDefaultAsync(k => k.Id == id);
        }

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
                foreach (var oldPre in kurs.Prerequisites.ToList())
                {
                    oldPre.IsPrerequisiteFor.Remove(kurs);
                }
                kurs.Prerequisites.Clear();

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

        public async Task DeleteKursAsync(int id)
        {
            var kurs = await _context.Kursy
                .Include(k => k.Prerequisites)
                .Include(k => k.IsPrerequisiteFor)
                .SingleOrDefaultAsync(k => k.Id == id);

            if (kurs == null) return;

            foreach (var pre in kurs.Prerequisites.ToList())
                pre.IsPrerequisiteFor.Remove(kurs);

            foreach (var dep in kurs.IsPrerequisiteFor.ToList()) 
                dep.Prerequisites.Remove(kurs);

            _context.Kursy.Remove(kurs);
            await _context.SaveChangesAsync();
        }
        
        public async Task<Kurs?> AddPrerequisiteAsync(int kursId, int prerequisiteId)
        {
            if (kursId == prerequisiteId)
                throw new InvalidOperationException("Nie można być prerekwizytem dla samego siebie");

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

            if (!kurs.Prerequisites.Contains(prerequisite))
            {
                kurs.Prerequisites.Add(prerequisite);
                prerequisite.IsPrerequisiteFor.Add(kurs);
                await _context.SaveChangesAsync();
            }

            return kurs;
        }
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
