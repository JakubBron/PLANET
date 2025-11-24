using Application.Interfaces;
using Domain.Entities;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class GabinetService : IGabinetService
    {
        private readonly UniversityDbContext _context;

        public GabinetService(UniversityDbContext context)
        {
            _context = context;
        }

        // CREATE
        public async Task<Gabinet> CreateGabinetAsync(string numerGabinetu, int? profesorId = null)
        {
            var gabinet = new Gabinet
            {
                NumerGabinetu = numerGabinetu,
                ProfesorId = profesorId
            };
            
            Profesor profesor = null;
            if (profesorId != null)
            {
                gabinet.ProfesorId = profesorId;
                profesor = _context.Profesorzy.SingleOrDefault(p => p.Id == profesorId);
                profesor.Gabinet = gabinet;
                gabinet.Profesor = profesor;
            }
            else
            {
                gabinet.ProfesorId = null;
            } 
            
            _context.Gabinety.Add(gabinet);
            await _context.SaveChangesAsync();
            return gabinet;
        }

        // READ ALL
        public async Task<List<Gabinet>> GetAllGabinetyAsync()
        {
            return await _context.Gabinety
                .Include(g => g.Profesor)
                .ToListAsync();
        }

        // READ BY ID
        public async Task<Gabinet?> GetGabinetByIdAsync(int id)
        {
            return await _context.Gabinety
                .Include(g => g.Profesor)
                .SingleOrDefaultAsync(g => g.Id == id);
        }

        // UPDATE
        public async Task<Gabinet?> UpdateGabinetAsync(int id, string? numerGabinetu = null, int? profesorId = null)
        {
            var gabinet = await _context.Gabinety.FindAsync(id);
            if (gabinet == null) return null;

            if (!string.IsNullOrWhiteSpace(numerGabinetu))
                gabinet.NumerGabinetu = numerGabinetu;
            if (profesorId.HasValue)
            {
                var profesor = await _context.Profesorzy.FindAsync(profesorId.Value);
                if (profesor == null)
                    throw new InvalidOperationException("Profesor nie istnieje.");
                gabinet.ProfesorId = profesorId;
                profesor.Gabinet = gabinet;
            }

            await _context.SaveChangesAsync();
            return gabinet;
        }

        // DELETE
        public async Task DeleteGabinetAsync(int id)
        {
            var gabinet = await _context.Gabinety.FindAsync(id);
            if (gabinet == null) return;

            var profesor = await _context.Profesorzy
                .FirstOrDefaultAsync(p => p.Gabinet == gabinet);
            if (profesor != null)
            {
                profesor.Gabinet = null;
            }

            _context.Gabinety.Remove(gabinet);
            await _context.SaveChangesAsync();
        }
    }
}
