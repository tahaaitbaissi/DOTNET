using CarRental.Core.Entities;
using CarRental.Core.Interfaces.Repositories;
using CarRental.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CarRental.Persistence.Repositories
{
    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Employee> GetByUserIdAsync(long userId)
        {
            return await _context.Set<Employee>()
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.UserId == userId);
        }

        public async Task<Employee> GetByEmailAsync(string email)
        {
            return await _context.Set<Employee>()
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.User.Email == email);
        }
    }
}
