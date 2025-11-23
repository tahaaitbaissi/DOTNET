using CarRental.Core.Interfaces;
using CarRental.Core.Interfaces.Repositories;
using CarRental.Persistence.Repositories;
using CarRental.Persistence.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CarRental.Persistence
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection") ??
                    "Server=(localdb)\\mssqllocaldb;Database=CarRentalDb;Trusted_Connection=True;MultipleActiveResultSets=true",
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            // Register Repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repositories.Base.Repository<>));
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IVehicleRepository, VehicleRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();

            // Register UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

            return services;
        }
    }
}

