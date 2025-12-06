using CarRental.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRental.Core.Interfaces.Repositories
{
    public interface IMaintenanceRepository : IRepository<Maintenance>
    {
        Task<IEnumerable<Maintenance>> GetActiveMaintenancesAsync();
        Task<IEnumerable<Maintenance>> GetByVehicleIdAsync(long vehicleId);
    }
}
