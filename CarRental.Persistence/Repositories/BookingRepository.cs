using CarRental.Core.Entities;
using CarRental.Core.Interfaces.Repositories;
using CarRental.Core.ValueObjects;
using CarRental.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarRental.Persistence.Repositories
{
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {
        public BookingRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<Booking> GetByIdAsync(long id)
        {
            return await _dbSet
                .Include(b => b.Vehicle)
                .Include(b => b.Client)
                    .ThenInclude(c => c.User)
                .Include(b => b.Payments)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Booking>> GetBookingsForClientAsync(long clientId)
        {
            return await _dbSet
                .Where(b => b.ClientId == clientId)
                .Include(b => b.Vehicle)
                .Include(b => b.Payments)
                .OrderByDescending(b => b.StartDate)
                .ToListAsync();
        }

        public async Task<bool> IsVehicleAvailableAsync(long vehicleId, DateRange range)
        {
            // Check for overlapping bookings (excluding cancelled and completed ones)
            // Two date ranges overlap if: StartDate < range.End AND range.Start < EndDate
            var hasOverlap = await _dbSet
                .Where(b => b.VehicleId == vehicleId &&
                           b.Status != Core.Enums.BookingStatus.Cancelled &&
                           b.Status != Core.Enums.BookingStatus.Completed &&
                           b.StartDate < range.End &&
                           range.Start < b.EndDate)
                .AnyAsync();

            return !hasOverlap;
        }

        public async Task<IEnumerable<Booking>> GetBookingsOverlappingWithDateRangeAsync(DateRange range)
        {
            return await _dbSet
                .Where(b => b.Status != Core.Enums.BookingStatus.Cancelled &&
                           b.Status != Core.Enums.BookingStatus.Completed &&
                           b.StartDate < range.End &&
                           range.Start < b.EndDate)
                .Include(b => b.Vehicle)
                .Include(b => b.Client)
                .ToListAsync();
        }
    }
}
