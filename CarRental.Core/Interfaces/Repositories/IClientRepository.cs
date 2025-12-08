using CarRental.Core.Entities;
using System.Threading.Tasks;

namespace CarRental.Core.Interfaces.Repositories
{
    public interface IClientRepository : IRepository<Client>
    {
        Task<Client> GetClientByUserIdAsync(long userId);
        Task<Client> GetClientByDriverLicenseAsync(string licenseNumber);
    }
}