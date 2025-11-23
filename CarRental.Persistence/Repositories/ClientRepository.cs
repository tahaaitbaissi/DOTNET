using CarRental.Core.Entities;
using CarRental.Core.Interfaces.Repositories;
using CarRental.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CarRental.Persistence.Repositories
{
    public class ClientRepository : Repository<Client>, IClientRepository
    {
        public ClientRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Client> GetClientByUserIdAsync(long userId)
        {
            return await _dbSet
                .Include(c => c.User)
                .Include(c => c.Bookings)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Client> GetClientByDriverLicenseAsync(string licenseNumber)
        {
            return await _dbSet
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.DriverLicense == licenseNumber);
        }
    }
}

