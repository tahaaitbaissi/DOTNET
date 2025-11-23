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

        public async Task<IEnumerable<Booking>> GetBookingsForClientAsync(long clientId)
        {
            return await _dbSet
                .Where(b => b.ClientId == clientId)
                .Include(b => b.Vehicle)
                .Include(b => b.Payments)
                .OrderByDescending(b => b.BookingTime)
                .ToListAsync();
        }

        public async Task<bool> IsVehicleAvailableAsync(long vehicleId, DateRange range)
        {
            // Check for overlapping bookings (excluding cancelled ones)
            // Using BookingTime as reference point - adjust based on actual business logic
            var hasOverlap = await _dbSet
                .Where(b => b.VehicleId == vehicleId &&
                           b.Status != Core.Enums.BookingStatus.Cancelled &&
                           b.Enable <= range.End &&
                           b.BookingTime >= range.Start)
                .AnyAsync();

            return !hasOverlap;
        }

        public async Task<IEnumerable<Booking>> GetBookingsOverlappingWithDateRangeAsync(DateRange range)
        {
            return await _dbSet
                .Where(b => b.Status != Core.Enums.BookingStatus.Cancelled &&
                           b.Enable <= range.End &&
                           b.BookingTime >= range.Start)
                .Include(b => b.Vehicle)
                .Include(b => b.Client)
                .ToListAsync();
        }
    }
}

