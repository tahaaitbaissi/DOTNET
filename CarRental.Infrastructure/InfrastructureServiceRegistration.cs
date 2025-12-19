using CarRental.Core.Interfaces.Services;
using CarRental.Application.Interfaces;
using CarRental.Infrastructure.Common;
using CarRental.Infrastructure.Security;
using CarRental.Infrastructure.Services;
using CarRental.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CarRental.Infrastructure
{
    /// <summary>
    /// Extension methods for registering Infrastructure layer services
    /// </summary>
    public static class InfrastructureServiceRegistration
    {
        /// <summary>
        /// Adds Infrastructure layer services to the DI container
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">The application configuration</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Register configuration settings
            services.Configure<EmailSettings>(configuration.GetSection(EmailSettings.SectionName));
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
            services.Configure<AppSettings>(configuration.GetSection(AppSettings.SectionName));

            // Register common services
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

            // Register document services
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IPdfService, PdfService>();
            services.AddScoped<IQrCodeService, QrCodeService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IExportService, CsvExportService>();
            services.AddScoped<IImportService, CsvImportService>();

            // Register security services
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ITokenService, TokenService>();

            return services;
        }
    }
}
