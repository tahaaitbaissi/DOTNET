using CarRental.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarRental.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets for all entities
        public DbSet<User> Users => Set<User>();
        public DbSet<Client> Clients => Set<Client>();
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Vehicle> Vehicles => Set<Vehicle>();
        public DbSet<VehicleType> VehicleTypes => Set<VehicleType>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Tariff> Tariffs => Set<Tariff>();
        public DbSet<Maintenance> MaintenanceRecords => Set<Maintenance>();
        public DbSet<Document> Documents => Set<Document>();
        public DbSet<VehicleImage> VehicleImages => Set<VehicleImage>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all configurations from the Configurations folder
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}

