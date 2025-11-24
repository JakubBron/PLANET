using Application.Interfaces;
using Domain.Entities;
using Domain.ValueObjects;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class ProfesorService: IProfesorService
    {
        private readonly UniversityDbContext _context;
        private readonly ILicznikIndeksowService _licznikIndeksowService;

        public ProfesorService(UniversityDbContext context, ILicznikIndeksowService licznikIndeksowService)
        {
            _context = context;
            _licznikIndeksowService = licznikIndeksowService;
        }

        // CREATE
        public async Task<Profesor> CreateProfesorAsync(string imie, string nazwisko, string tytulNaukowy, Adres adres, int wydzialId, int? gabinetId, string prefix = "P")
        {
            //using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                int numer = await _licznikIndeksowService.GetNextAsync(prefix);
                string nowyIndeks = $"{prefix}{numer}";


                var profesor = new Profesor
                {
                    Imie = imie,
                    Nazwisko = nazwisko,
                    TytulNaukowy = tytulNaukowy,
                    AdresZamieszkania = adres,
                    WydzialId = wydzialId,
                    IndeksUczelniany = nowyIndeks,
                };
                if (gabinetId.HasValue)
                {
                    var gabinet = await _context.Gabinety.FindAsync(gabinetId.Value);
                    if (gabinet == null)
                        throw new InvalidOperationException($"Gabinet o Id={gabinetId} nie istnieje.");
                    profesor.Gabinet = gabinet;
                    gabinet.Profesor = profesor;
                    gabinet.ProfesorId = profesor.Id;
                }

                _context.Profesorzy.Add(profesor);
                await _context.SaveChangesAsync();
                //await transaction.CommitAsync();
                return profesor;
            }
            catch(Exception e)
            {
                //transaction.Rollback();
                throw e;
            }
        }

        // READ ALL
        public async Task<List<Profesor>> GetAllProfesorzyAsync()
        {
            return await _context.Profesorzy
                .Include(p => p.AdresZamieszkania)
                .Include(p => p.Wydzial)
                .ToListAsync();
        }

        // READ BY ID
        public async Task<Profesor?> GetProfesorByIdAsync(int id)
        {
            return await _context.Profesorzy
                .Include(p => p.AdresZamieszkania)
                .Include(p => p.Wydzial)
                .SingleOrDefaultAsync(p => p.Id == id);
        }

        // UPDATE
        public async Task<Profesor?> UpdateProfesorAsync(int id, string? imie = null, string? nazwisko = null, string? tytulNaukowy = null, Adres? adres = null, int? wydzialId = null, int? gabinetId = null)
        {
            var profesor = await _context.Profesorzy.FindAsync(id);
            if (profesor == null) return null;

            if (!string.IsNullOrWhiteSpace(imie)) profesor.Imie = imie;
            if (!string.IsNullOrWhiteSpace(nazwisko)) profesor.Nazwisko = nazwisko;
            if (!string.IsNullOrWhiteSpace(tytulNaukowy)) profesor.TytulNaukowy = tytulNaukowy;
            if (adres != null) profesor.AdresZamieszkania = adres;
            if (wydzialId.HasValue)
            {
                var wydzial = await _context.Gabinety.FindAsync(wydzialId.Value);
                if (wydzial == null)
                    throw new InvalidOperationException($"Wydział o Id={wydzialId} nie istnieje.");
                profesor.WydzialId = wydzialId.Value;
            }
            if (gabinetId.HasValue)
            {
                var gabinet = await _context.Gabinety.FindAsync(gabinetId.Value);
                if (gabinet == null)
                    throw new InvalidOperationException($"Gabinet o Id={gabinetId} nie istnieje.");
                profesor.Gabinet = gabinet;
            }

            await _context.SaveChangesAsync();
            return profesor;
        }

        // DELETE
        public async Task DeleteProfesorAsync(int id)
        {
            var profesor = await _context.Profesorzy.FindAsync(id);
            if (profesor == null) return;
            
            var gabinet = await _context.Gabinety.SingleOrDefaultAsync(g => g.ProfesorId == profesor.Id);
            if (gabinet != null)
            {
                gabinet.ProfesorId = null;
                gabinet.Profesor = null;
            }

            var numer = profesor.IndeksUczelniany;
            if (numer != null)
            {
                _licznikIndeksowService.DecrementIfLast(numer);
            }

            
            

            _context.Profesorzy.Remove(profesor);
            await _context.SaveChangesAsync();
        }
    }
}
