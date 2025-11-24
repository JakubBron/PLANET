using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ILicznikIndeksowService
    {
        Task<int> GetNextAsync(string prefix);                  // Pobranie i inkrementacja
        Task<LicznikIndeksow> CreatePrefixAsync(string prefix, int startValue = 0); // Tworzenie nowego prefiksu
        Task<LicznikIndeksow?> GetAsync(string prefix);         // Pobranie pojedynczego
        Task<List<LicznikIndeksow>> GetAllAsync();             // Pobranie wszystkich
        Task<LicznikIndeksow?> UpdateAsync(string prefix, int newValue); // Aktualizacja wartości
        Task DeleteAsync(string prefix);                        // Usunięcie prefiksu
        Task DecrementIfLast(string numer);                     // Dekrementacja jeśli ostatni
    }
}