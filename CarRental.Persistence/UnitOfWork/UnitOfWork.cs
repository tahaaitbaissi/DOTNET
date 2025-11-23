using CarRental.Core.Interfaces;
using CarRental.Core.Interfaces.Repositories;
using CarRental.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading.Tasks;

namespace CarRental.Persistence.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction _transaction;

        private IBookingRepository _bookings;
        private IVehicleRepository _vehicles;
        private IClientRepository _clients;

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

