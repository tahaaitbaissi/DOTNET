using CarRental.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRental.Core.Interfaces.Repositories
{
    public interface IRepository<T> where T : Entity
    {
        Task<T> GetByIdAsync(long id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<int> CountAsync();
    }
}