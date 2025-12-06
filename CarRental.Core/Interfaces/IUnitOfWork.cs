using CarRental.Core.Entities;
using CarRental.Core.Interfaces.Repositories;
using System;
using System.Threading.Tasks;

namespace CarRental.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IBookingRepository Bookings { get; }
        IVehicleRepository Vehicles { get; }
        IClientRepository Clients { get; }
        IUserRepository Users { get; }
        IRepository<VehicleImage> VehicleImages { get; }
        IRepository<VehicleType> VehicleTypes { get; }

        IPaymentRepository Payments { get; }
        IEmployeeRepository Employees { get; }
        IMaintenanceRepository Maintenances { get; }
        INotificationRepository Notifications { get; }

        Task<int> SaveChangesAsync();
    }
}