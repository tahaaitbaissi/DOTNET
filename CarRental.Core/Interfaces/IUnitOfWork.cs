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

        Task<int> SaveChangesAsync();
    }
}