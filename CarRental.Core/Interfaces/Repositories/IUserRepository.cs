using CarRental.Core.Entities;
using System.Threading.Tasks;

namespace CarRental.Core.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByUsernameAsync(string username);
        Task<bool> ExistsWithEmailAsync(string email);
        Task<bool> ExistsWithUsernameAsync(string username);
    }
}

