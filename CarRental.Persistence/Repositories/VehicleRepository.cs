using CarRental.Core.Entities;
using CarRental.Core.Enums;
using CarRental.Core.Interfaces.Repositories;
using CarRental.Core.ValueObjects;
using CarRental.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarRental.Persistence.Repositories
{
    public class VehicleRepository : Repository<Vehicle>, IVehicleRepository
    {
        public VehicleRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync(DateRange range, long? vehicleTypeId)
        {
            var query = _dbSet
                .Where(v => v.Status == VehicleStatus.Available);

            // Filter by vehicle type if specified (using shadow property)
            if (vehicleTypeId.HasValue)
            {
                query = query.Where(v => EF.Property<long?>(v, "VehicleTypeId") == vehicleTypeId.Value);
            }

            // Exclude vehicles that have overlapping bookings
            // Two date ranges overlap if: StartDate < range.End AND range.Start < EndDate
            var unavailableVehicleIds = await _context.Bookings
                .Where(b => b.Status != BookingStatus.Cancelled &&
                           b.Status != BookingStatus.Completed &&
                           b.StartDate < range.End &&
                           range.Start < b.EndDate)
                .Select(b => b.VehicleId)
                .Distinct()
                .ToListAsync();

            query = query.Where(v => !unavailableVehicleIds.Contains(v.Id));

            return await query
                .Include(v => v.Images)
                .ToListAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesNeedingMaintenanceAsync()
        {
            return await _dbSet
                .Where(v => v.Status == VehicleStatus.InMaintenance ||
                           v.MaintenanceHistory.Any(m => !m.IsCompleted))
                .Include(v => v.MaintenanceHistory.Where(m => !m.IsCompleted))
                .ToListAsync();
        }

        public async Task<Vehicle?> GetByVinAsync(string vin)
        {
            return await _dbSet
                .FirstOrDefaultAsync(v => v.VIN == vin);
        }
    }
}

