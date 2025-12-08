using CarRental.Core.Entities;
using System.Threading.Tasks;

namespace CarRental.Core.Interfaces.Repositories
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task<Employee> GetByUserIdAsync(long userId);
        Task<Employee> GetByEmailAsync(string email);
    }
}
