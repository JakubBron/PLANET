using Application.Interfaces;
using Domain.Entities;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class WydzialService : IWydzialService
    {
        private readonly UniversityDbContext _context;

        public WydzialService(UniversityDbContext context)
        {
            _context = context;
        }

        public async Task<Wydzial> CreateWydzialAsync(string nazwa)
        {
            var wydzial = new Wydzial
            {
                Nazwa = nazwa
            };

            _context.Wydzialy.Add(wydzial);
            await _context.SaveChangesAsync();
            return wydzial;
        }

        public async Task<List<Wydzial>> GetAllWydzialyAsync()
        {
            return await _context.Wydzialy
                .Include(w => w.Profesorzy)
                .Include(w => w.Kursy)
                .ToListAsync();
        }
        public async Task<Wydzial?> GetWydzialByIdAsync(int id)
        {
            return await _context.Wydzialy
                .Include(w => w.Profesorzy)
                .Include(w => w.Kursy)
                .SingleOrDefaultAsync(w => w.Id == id);
        }

        public async Task<Wydzial?> UpdateWydzialAsync(int id, string? nazwa = null)
        {
            var wydzial = await _context.Wydzialy.FindAsync(id);
            if (wydzial == null) return null;

            if (!string.IsNullOrWhiteSpace(nazwa)) wydzial.Nazwa = nazwa;

            await _context.SaveChangesAsync();
            return wydzial;
        }

        public async Task DeleteWydzialAsync(int id)
        {
            try
            {
                var wydzial = await _context.Wydzialy.FindAsync(id);
                if (wydzial == null) return;

                _context.Wydzialy.Remove(wydzial);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new Exception($"Na tym wydziale istnieją kursy! Nie można usunąć. Exception = {e}");
            }

        }
    }
}