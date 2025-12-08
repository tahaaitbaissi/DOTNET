using CarRental.Core.Entities;
using CarRental.Core.Interfaces.Repositories;
using CarRental.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarRental.Persistence.Repositories
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        public PaymentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Payment>> GetByBookingIdAsync(long bookingId)
        {
            return await _context.Set<Payment>()
                .Where(p => p.BookingId == bookingId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }
    }
}
