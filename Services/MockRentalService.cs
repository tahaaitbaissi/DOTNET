using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarRental.Desktop.Models;

namespace CarRental.Desktop.Services
{
    public interface IRentalService
    {
        Task<List<Rental>> GetAllRentalsAsync();
    }

    public class MockRentalService : IRentalService
    {
        private readonly List<Rental> _rentals;

        public MockRentalService()
        {
            _rentals = new List<Rental>
            {
                new Rental { Id = 101, CustomerName = "John Doe", VehicleModel = "Toyota Camry", StartDate = DateTime.Now.AddDays(-2), EndDate = DateTime.Now.AddDays(1), Status = "Active", TotalAmount = 150 },
                new Rental { Id = 102, CustomerName = "Jane Smith", VehicleModel = "Ford Focus", StartDate = DateTime.Now.AddDays(-5), EndDate = DateTime.Now.AddDays(-1), Status = "Completed", TotalAmount = 180 },
                new Rental { Id = 103, CustomerName = "Bob Johnson", VehicleModel = "Honda CR-V", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(3), Status = "Active", TotalAmount = 195 },
                new Rental { Id = 104, CustomerName = "Alice Brown", VehicleModel = "Tesla Model 3", StartDate = DateTime.Now.AddDays(-1), EndDate = DateTime.Now.AddDays(2), Status = "Active", TotalAmount = 270 },
                new Rental { Id = 105, CustomerName = "Charlie Davis", VehicleModel = "Chevrolet Tahoe", StartDate = DateTime.Now.AddDays(-10), EndDate = DateTime.Now.AddDays(-3), Status = "Completed", TotalAmount = 840 },
                new Rental { Id = 106, CustomerName = "Eva Green", VehicleModel = "BMW X5", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(5), Status = "Pending", TotalAmount = 750 }
            };
        }

        public Task<List<Rental>> GetAllRentalsAsync()
        {
            return Task.FromResult(_rentals.ToList());
        }
    }
}
