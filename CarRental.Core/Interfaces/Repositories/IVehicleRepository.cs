using CarRental.Core.Entities;
using CarRental.Core.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRental.Core.Interfaces.Repositories
{
    public interface IVehicleRepository : IRepository<Vehicle>
    {
        Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync(DateRange range, long? vehicleTypeId);
        Task<IEnumerable<Vehicle>> GetVehiclesNeedingMaintenanceAsync();
    }
}