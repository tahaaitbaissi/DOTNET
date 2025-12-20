using CarRental.Core.Entities;
using CarRental.Core.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRental.Core.Interfaces.Repositories
{
    public interface IBookingRepository : IRepository<Booking>
    {
        Task<IEnumerable<Booking>> GetBookingsForClientAsync(long clientId);
        Task<bool> IsVehicleAvailableAsync(long vehicleId, DateRange range);
        Task<IEnumerable<Booking>> GetBookingsOverlappingWithDateRangeAsync(DateRange range);
        Task<IEnumerable<Booking>> GetAllWithDetailsAsync();
    }
}