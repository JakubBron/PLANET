using Domain.Entities;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Interfaces;
using Humanizer;
using Microsoft.EntityFrameworkCore.Internal;

namespace Application.Services
{
    public class LicznikIndeksowService : ILicznikIndeksowService
    {
        private readonly UniversityDbContext _context;

        public LicznikIndeksowService(UniversityDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetNextAsync(string prefix)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            var licznik = await _context.LicznikiIndeksow.SingleOrDefaultAsync(l => l.Prefix == prefix);

            if (licznik == null)
            {
                licznik = new LicznikIndeksow
                {
                    Prefix = prefix,
                    AktualnaWartosc = 1
                };
                _context.LicznikiIndeksow.Add(licznik);
            }
            else
            {
                licznik.AktualnaWartosc++;
                _context.LicznikiIndeksow.Update(licznik);
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return licznik.AktualnaWartosc;
        }

        public async Task<LicznikIndeksow> CreatePrefixAsync(string prefix, int startValue = 0)
        {
            if (await _context.LicznikiIndeksow.AnyAsync(l => l.Prefix == prefix))
                throw new InvalidOperationException($"Prefiks '{prefix}' już istnieje.");

            var licznik = new LicznikIndeksow
            {
                Prefix = prefix,
                AktualnaWartosc = startValue
            };

            _context.LicznikiIndeksow.Add(licznik);
            await _context.SaveChangesAsync();

            return licznik;
        }

        public async Task<LicznikIndeksow?> GetAsync(string prefix)
        {
            return await _context.LicznikiIndeksow.SingleOrDefaultAsync(l => l.Prefix == prefix);
        }

        public async Task<List<LicznikIndeksow>> GetAllAsync()
        {
            return await _context.LicznikiIndeksow.ToListAsync();
        }

        public async Task<LicznikIndeksow?> UpdateAsync(string prefix, int newValue)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var licznik = await _context.LicznikiIndeksow.SingleOrDefaultAsync(l => l.Prefix == prefix);
                if (licznik == null)
                    throw new InvalidOperationException($"Prefiks '{prefix}' nie istnieje.");

                if (licznik.AktualnaWartosc > newValue)
                    throw new InvalidOperationException("Nie można ustawić wartości mniejszej niż aktualny stan licznika");

                licznik.AktualnaWartosc = newValue;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return licznik;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAsync(string prefix)
        {
            var licznik = await _context.LicznikiIndeksow.SingleOrDefaultAsync(l => l.Prefix == prefix);

            if (licznik == null) return;
            var students = await _context.Studenci
                .Where(s => s.IndeksUczelniany.StartsWith(prefix))
                .ToListAsync();
            var professors = await _context.Profesorzy
                .Where(p => p.IndeksUczelniany.StartsWith(prefix))
                .ToListAsync();

            if (students.Count > 0 || professors.Count > 0)
            {
                throw new InvalidOperationException("Nie można usunąć tego licznika! Licznik był już wykorzystany");
            }

            _context.LicznikiIndeksow.Remove(licznik);
            await _context.SaveChangesAsync();
        }

        public async Task DecrementIfLast(string numer)
        {
            var prefix = new string(numer.TakeWhile(char.IsLetter).ToArray());
            var liczbaStr = new string(numer.SkipWhile(char.IsLetter).ToArray());
            if (!int.TryParse(liczbaStr, out var liczba))
                throw new InvalidOperationException($"Numer '{numer}' nie zawiera poprawnej liczby po prefiksie.");

            var licznik = await _context.LicznikiIndeksow.SingleOrDefaultAsync(l => l.Prefix == prefix);
            if (licznik == null)
                throw new InvalidOperationException($"Prefiks '{prefix}' nie istnieje.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {

                if (licznik.AktualnaWartosc == liczba)
                {
                    licznik.AktualnaWartosc--;
                }
                transaction.CommitAsync();
            }
            catch(Exception e)
            {
                transaction.Rollback();
                throw e;
            }
        }
    }
}
