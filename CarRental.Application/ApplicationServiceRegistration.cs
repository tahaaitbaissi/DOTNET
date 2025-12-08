using CarRental.Application.Interfaces;
using CarRental.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CarRental.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register Application Services
            services.AddScoped<IBookingService, BookingService>();
            services.AddScoped<IVehicleService, VehicleService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IVehicleTypeService, VehicleTypeService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IMaintenanceService, MaintenanceService>();
            services.AddScoped<IDashboardService, DashboardService>();

            // ExportService is implemented in Infrastructure and should be registered there.

            
            services.AddScoped<ITariffService, TariffService>();
            
            return services;
        }
    }
}

