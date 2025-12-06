using CarRental.Core.Entities;
using CarRental.Core.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CarRental.Core.Interfaces.Repositories
{
    public interface INotificationRepository : IRepository<Notification>
    {
        Task<IEnumerable<Notification>> GetByUserIdAsync(long userId);
        Task<IEnumerable<Notification>> GetUnreadAsync(long userId);
    }
}
