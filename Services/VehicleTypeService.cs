using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarRental.Desktop.Models;

namespace CarRental.Desktop.Services
{
    public interface IVehicleTypeService
    {
        Task<List<VehicleType>> GetAllVehicleTypesAsync();
        Task AddVehicleTypeAsync(VehicleType vehicleType);
        Task UpdateVehicleTypeAsync(VehicleType vehicleType);
        Task DeleteVehicleTypeAsync(int id);
    }

    public class MockVehicleTypeService : IVehicleTypeService
    {
        private readonly List<VehicleType> _vehicleTypes;

        public MockVehicleTypeService()
        {
            _vehicleTypes = new List<VehicleType>
            {
                new VehicleType { Id = 1, Name = "Compact", Description = "Small and fuel efficient.", BasePrice = 30 },
                new VehicleType { Id = 2, Name = "Sedan", Description = "Comfortable for 4 passengers.", BasePrice = 50 },
                new VehicleType { Id = 3, Name = "SUV", Description = "Spacious and powerful.", BasePrice = 80 },
                new VehicleType { Id = 4, Name = "Luxury", Description = "Premium experience.", BasePrice = 150 },
                new VehicleType { Id = 5, Name = "Van", Description = "For large groups/cargo.", BasePrice = 100 }
            };
        }

        public Task<List<VehicleType>> GetAllVehicleTypesAsync()
        {
            return Task.FromResult(_vehicleTypes.ToList());
        }

        public Task AddVehicleTypeAsync(VehicleType vehicleType)
        {
            if (_vehicleTypes.Any())
                vehicleType.Id = _vehicleTypes.Max(vt => vt.Id) + 1;
            else
                vehicleType.Id = 1;

            _vehicleTypes.Add(vehicleType);
            return Task.CompletedTask;
        }

        public Task UpdateVehicleTypeAsync(VehicleType vehicleType)
        {
            var existing = _vehicleTypes.FirstOrDefault(vt => vt.Id == vehicleType.Id);
            if (existing != null)
            {
                existing.Name = vehicleType.Name;
                existing.Description = vehicleType.Description;
                existing.BasePrice = vehicleType.BasePrice;
            }
            return Task.CompletedTask;
        }

        public Task DeleteVehicleTypeAsync(int id)
        {
            var existing = _vehicleTypes.FirstOrDefault(vt => vt.Id == id);
            if (existing != null)
            {
                _vehicleTypes.Remove(existing);
            }
            return Task.CompletedTask;
        }
    }
}
