using CarRental.Core.Entities;
using CarRental.Core.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using CarRental.Core.Interfaces.Services;

namespace CarRental.Persistence.Seed
{
    public class DatabaseSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public DatabaseSeeder(ApplicationDbContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task SeedAsync()
        {
            // Ensure database is created
            await _context.Database.EnsureCreatedAsync();

            // Seed VehicleTypes
            if (!await _context.VehicleTypes.AnyAsync())
            {
                await SeedVehicleTypesAsync();
            }

            // Seed Roles
            if (!await _context.Roles.AnyAsync())
            {
                await SeedRolesAsync();
                // Save roles so we can reference their generated IDs
                await _context.SaveChangesAsync();
            }

            // Seed Admin User
            if (!await _context.Users.AnyAsync(u => u.Email == "admin@carrental.com"))
            {
                await SeedAdminUserAsync();
            }

            await _context.SaveChangesAsync();
        }

        private async Task SeedVehicleTypesAsync()
        {
            var vehicleTypes = new[]
            {
                new VehicleType { Name = "Economy", Description = "Small, fuel-efficient cars", BaseRate = 30.00m },
                new VehicleType { Name = "Compact", Description = "Mid-size cars", BaseRate = 40.00m },
                new VehicleType { Name = "Standard", Description = "Full-size cars", BaseRate = 50.00m },
                new VehicleType { Name = "Premium", Description = "Luxury cars", BaseRate = 80.00m },
                new VehicleType { Name = "SUV", Description = "Sport Utility Vehicles", BaseRate = 70.00m },
                new VehicleType { Name = "Van", Description = "Large passenger vans", BaseRate = 90.00m }
            };

            await _context.VehicleTypes.AddRangeAsync(vehicleTypes);
        }

        private async Task SeedRolesAsync()
        {
            var roles = new[]
            {
                new Role { RoleName = UserRole.Client, Description = "Regular client role" },
                new Role { RoleName = UserRole.Employee, Description = "Employee role" },
                new Role { RoleName = UserRole.User, Description = "Basic user role" }
            };

            await _context.Roles.AddRangeAsync(roles);
        }

        private async Task SeedAdminUserAsync()
        {
            // Create admin user with hashed default password
            var password = "Admin@123"; // default password for local development
            var hashed = _passwordHasher.HashPassword(password);

            var adminUser = new User
            {
                Email = "admin@carrental.com",
                Username = "admin",
                FullName = "System Administrator",
                PasswordHash = hashed,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Assign Employee role if available
            var employeeRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == UserRole.Employee);
            if (employeeRole != null)
            {
                adminUser.RoleId = employeeRole.Id;
            }

            await _context.Users.AddAsync(adminUser);
            await _context.SaveChangesAsync(); // Save to get the User ID

            // Create corresponding employee record
            var adminEmployee = new Employee
            {
                UserId = adminUser.Id,
                Position = "Administrator",
                HireDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Employees.AddAsync(adminEmployee);
        }
    }
}

