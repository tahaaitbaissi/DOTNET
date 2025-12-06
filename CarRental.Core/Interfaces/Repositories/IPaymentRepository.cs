using CarRental.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRental.Core.Interfaces.Repositories
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<IEnumerable<Payment>> GetByBookingIdAsync(long bookingId);
    }
}
