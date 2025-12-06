using CarRental.Core.Entities;
using CarRental.Core.Interfaces.Repositories;
using CarRental.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarRental.Persistence.Repositories
{
    // Need interface first? I'll use generic repo in UoW for simple needs, 
    // but a specific repo is better for complex queries like "GetActiveMaintenance".
    // I'll define interface inline in plan realization if not pre-made.
    // Plan said: [NEW] IMaintenanceRepository
    
    public class MaintenanceRepository : Repository<Maintenance>, IMaintenanceRepository
    {
        public MaintenanceRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Maintenance>> GetActiveMaintenancesAsync()
        {
            return await _context.Set<Maintenance>()
                .Include(m => m.Vehicle)
                .Where(m => !m.IsCompleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<Maintenance>> GetByVehicleIdAsync(long vehicleId)
        {
             return await _context.Set<Maintenance>()
                .Where(m => m.VehicleId == vehicleId)
                .OrderByDescending(m => m.StartDate)
                .ToListAsync();
        }
    }
}
