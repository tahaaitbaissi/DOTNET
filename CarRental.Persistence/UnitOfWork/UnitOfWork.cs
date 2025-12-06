using CarRental.Core.Interfaces;
using CarRental.Core.Interfaces.Repositories;
using CarRental.Persistence.Repositories;
using CarRental.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore.Storage;
using CarRental.Core.Entities;
using System;
using System.Threading.Tasks;

namespace CarRental.Persistence.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _transaction;

        private IBookingRepository? _bookings;
        private IVehicleRepository? _vehicles;
        private IClientRepository? _clients;
        private IUserRepository? _users;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IBookingRepository Bookings
        {
            get
            {
                return _bookings ??= new BookingRepository(_context);
            }
        }

        public IVehicleRepository Vehicles
        {
            get
            {
                return _vehicles ??= new VehicleRepository(_context);
            }
        }

        public IClientRepository Clients
        {
            get
            {
                return _clients ??= new ClientRepository(_context);
            }
        }

        public IUserRepository Users
        {
            get
            {
                return _users ??= new UserRepository(_context);
            }
        }

        private IRepository<VehicleImage>? _vehicleImages;
        public IRepository<VehicleImage> VehicleImages
        {
            get
            {
                return _vehicleImages ??= new Repository<VehicleImage>(_context);
            }
        }

        private IRepository<VehicleType>? _vehicleTypes;
        public IRepository<VehicleType> VehicleTypes
        {
            get
            {
                return _vehicleTypes ??= new Repository<VehicleType>(_context);
            }
        }

        private IPaymentRepository? _payments;
        public IPaymentRepository Payments
        {
            get
            {
                return _payments ??= new PaymentRepository(_context);
            }
        }

        private IEmployeeRepository? _employees;
        public IEmployeeRepository Employees
        {
            get
            {
                return _employees ??= new EmployeeRepository(_context);
            }
        }

        private IMaintenanceRepository? _maintenances;
        public IMaintenanceRepository Maintenances
        {
            get
            {
                return _maintenances ??= new MaintenanceRepository(_context);
            }
        }

        private INotificationRepository? _notifications;
        public INotificationRepository Notifications
        {
            get
            {
                return _notifications ??= new NotificationRepository(_context);
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                }
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context?.Dispose();
        }
    }
}

