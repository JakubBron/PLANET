using Domain.Entities;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Interfaces;

namespace Application.Services
{
    public class LicznikIndeksowService : ILicznikIndeksowService
    {
        private readonly UniversityDbContext _context;

        public LicznikIndeksowService(UniversityDbContext context)
        {
            _context = context;
        }

        // Pobranie kolejnego numeru (i inkrementacja)
        public async Task<int> GetNextAsync(string prefix)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            var licznik = await _context.LicznikiIndeksow
                .SingleOrDefaultAsync(l => l.Prefix == prefix);

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

        // Tworzenie nowego prefiksu
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

        // Pobranie pojedynczego prefiksu
        public async Task<LicznikIndeksow?> GetAsync(string prefix)
        {
            return await _context.LicznikiIndeksow
                .SingleOrDefaultAsync(l => l.Prefix == prefix);
        }

        // Pobranie wszystkich prefiksów
        public async Task<List<LicznikIndeksow>> GetAllAsync()
        {
            return await _context.LicznikiIndeksow.ToListAsync();
        }

        // Aktualizacja wartości licznika
        public async Task<LicznikIndeksow?> UpdateAsync(string oldPrefix, string newPrefix, int? newValue = null)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var licznik = await _context.LicznikiIndeksow.SingleOrDefaultAsync(l => l.Prefix == oldPrefix);
                if (licznik == null)
                    throw new InvalidOperationException($"Prefiks '{oldPrefix}' nie istnieje.");

                // 2. Sprawdzenie, czy nowy prefiks już nie istnieje
                var exists = await _context.LicznikiIndeksow.AnyAsync(l => l.Prefix == newPrefix);
                if (exists)
                    throw new InvalidOperationException($"Prefiks '{newPrefix}' już istnieje.");

                // 3. Aktualizacja studentów (lub profesorów) — zakładamy, że tylko studentów dla "S"
                var students = await _context.Studenci
                    .Where(s => s.IndeksUczelniany.StartsWith(oldPrefix))
                    .ToListAsync();
                var professors = await _context.Profesorzy
                    .Where(p => p.IndeksUczelniany.StartsWith(oldPrefix))
                    .ToListAsync();

                if ((students.Count > 0 || professors.Count > 0) && newValue != null)
                {
                    throw new InvalidOperationException(
                        "Nie można ustawić nowej wartości tego prefixu! Ten licznik był aktywny w przeszłości.");
                }

                else if (newValue != null)
                {
                    licznik.AktualnaWartosc = newValue.Value;
                }

                foreach (var student in students)
                {
                    student.IndeksUczelniany = newPrefix + student.IndeksUczelniany.Substring(oldPrefix.Length);
                }

                foreach (var prof in professors)
                {
                    prof.IndeksUczelniany = newPrefix + prof.IndeksUczelniany.Substring(oldPrefix.Length);
                }

                // 4. Zmiana prefiksu w liczniku
                licznik.Prefix = newPrefix;

                // 5. Zapis zmian i commit transakcji
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

        // Usunięcie prefiksu
        public async Task DeleteAsync(string prefix)
        {
            var licznik = await _context.LicznikiIndeksow
                .SingleOrDefaultAsync(l => l.Prefix == prefix);

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

        // Dekrementacja jeśli ostatni
        public async Task DecrementIfLast(string numer)
        {
            var prefix = new string(numer.TakeWhile(char.IsLetter).ToArray());
            var liczbaStr = new string(numer.SkipWhile(char.IsLetter).ToArray());
            if (!int.TryParse(liczbaStr, out var liczba))
                throw new InvalidOperationException($"Numer '{numer}' nie zawiera poprawnej liczby po prefiksie.");

            var licznik = await _context.LicznikiIndeksow
                .SingleOrDefaultAsync(l => l.Prefix == prefix);
            if (licznik == null)
                throw new InvalidOperationException($"Prefiks '{prefix}' nie istnieje.");
            
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {

                if (licznik.AktualnaWartosc == liczba)
                {
                    licznik.AktualnaWartosc--;
                    await _context.SaveChangesAsync();
                }
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
