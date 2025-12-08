using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarRental.Desktop.Models;

namespace CarRental.Desktop.Services
{
    public interface IVehicleService
    {
        Task<List<Vehicle>> GetAllVehiclesAsync();
        Task AddVehicleAsync(Vehicle vehicle);
    }

    public class MockVehicleService : IVehicleService
    {
        private readonly List<Vehicle> _vehicles;

        public MockVehicleService()
        {
            _vehicles = new List<Vehicle>
            {
                new Vehicle { Id = 1, Make = "Toyota", Model = "Camry", Year = 2023, LicensePlate = "ABC-123", Status = "Available", DailyRate = 50, ImagePath = "" },
                new Vehicle { Id = 2, Make = "Ford", Model = "Focus", Year = 2022, LicensePlate = "XYZ-789", Status = "Rented", DailyRate = 45, ImagePath = "" },
                new Vehicle { Id = 3, Make = "Honda", Model = "CR-V", Year = 2024, LicensePlate = "SUV-001", Status = "Available", DailyRate = 65, ImagePath = "" },
                new Vehicle { Id = 4, Make = "Tesla", Model = "Model 3", Year = 2023, LicensePlate = "ELN-420", Status = "Maintenance", DailyRate = 90, ImagePath = "" },
                new Vehicle { Id = 5, Make = "Chevrolet", Model = "Tahoe", Year = 2023, LicensePlate = "LGE-999", Status = "Available", DailyRate = 120, ImagePath = "" },
                new Vehicle { Id = 6, Make = "BMW", Model = "X5", Year = 2024, LicensePlate = "LUX-888", Status = "Rented", DailyRate = 150, ImagePath = "" },
                new Vehicle { Id = 7, Make = "Audi", Model = "A4", Year = 2022, LicensePlate = "BST-111", Status = "Available", DailyRate = 80, ImagePath = "" },
                new Vehicle { Id = 8, Make = "Mercedes", Model = "C-Class", Year = 2023, LicensePlate = "BENZ-001", Status = "Maintenance", DailyRate = 95, ImagePath = "" },
                new Vehicle { Id = 9, Make = "Jeep", Model = "Wrangler", Year = 2021, LicensePlate = "OFF-444", Status = "Available", DailyRate = 110, ImagePath = "" },
                new Vehicle { Id = 10, Make = "Nissan", Model = "Altima", Year = 2020, LicensePlate = "ECO-222", Status = "Available", DailyRate = 40, ImagePath = "" }
            };
        }

        public Task<List<Vehicle>> GetAllVehiclesAsync()
        {
            return Task.FromResult(_vehicles.ToList());
        }

        public Task AddVehicleAsync(Vehicle vehicle)
        {
            vehicle.Id = _vehicles.Max(v => v.Id) + 1;
            _vehicles.Add(vehicle);
            return Task.CompletedTask;
        }
    }
}
