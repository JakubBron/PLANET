using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ILicznikIndeksowService
    {
        Task<int> GetNextAsync(string prefix);
        Task<LicznikIndeksow> CreatePrefixAsync(string prefix, int startValue = 0);
        Task<LicznikIndeksow?> GetAsync(string prefix);
        Task<List<LicznikIndeksow>> GetAllAsync();
        Task<LicznikIndeksow?> UpdateAsync(string prefix, int newValue);
        Task DeleteAsync(string prefix);
        Task DecrementIfLast(string numer);
    }
}