using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace CarRental.Desktop.Services
{
    public interface IVehicleService
    {
        Task<List<CarRental.Application.DTOs.VehicleDto>> GetAllVehiclesAsync();
        Task AddVehicleAsync(CarRental.Application.DTOs.CreateVehicleDto vehicle);
    }

    public class MockVehicleService : IVehicleService
    {
        private readonly List<CarRental.Application.DTOs.VehicleDto> _vehicles;

        public MockVehicleService()
        {
            _vehicles = new List<CarRental.Application.DTOs.VehicleDto>
            {
                new CarRental.Application.DTOs.VehicleDto { Id = 1, Make = "Toyota", Model = "Camry", Year = 2023, LicensePlate = "ABC-123", Status = "Available", DailyRate = 50 },
                new CarRental.Application.DTOs.VehicleDto { Id = 2, Make = "Ford", Model = "Focus", Year = 2022, LicensePlate = "XYZ-789", Status = "Rented", DailyRate = 45 },
            };
        }

        public Task<List<CarRental.Application.DTOs.VehicleDto>> GetAllVehiclesAsync()
        {
            return Task.FromResult(_vehicles.ToList());
        }

        public Task AddVehicleAsync(CarRental.Application.DTOs.CreateVehicleDto vehicle)
        {
            // Mock addition
            var newVehicle = new CarRental.Application.DTOs.VehicleDto
            {
                Id = _vehicles.Any() ? _vehicles.Max(v => v.Id) + 1 : 1,
                Make = vehicle.Make,
                Model = vehicle.Model,
                Year = vehicle.Year,
                LicensePlate = vehicle.LicensePlate,
                Status = vehicle.Status.ToString(),
                DailyRate = vehicle.PricePerDay
            };
            _vehicles.Add(newVehicle);
            return Task.CompletedTask;
        }
    }
}
