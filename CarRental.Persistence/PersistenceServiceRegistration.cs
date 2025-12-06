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
            // Get connection string
            var connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? "Server=localhost;Database=CarRentalDb;User=root;Password=123;";

            // Register DbContext with MySQL
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            // Register Repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repositories.Base.Repository<>));
            services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IVehicleRepository, VehicleRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            // Register UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();

            return services;
        }
    }
}

