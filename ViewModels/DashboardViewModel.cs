using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CarRental.Desktop.Models;
using CarRental.Desktop.Services;

namespace CarRental.Desktop.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private readonly IVehicleService _vehicleService;
        private readonly IRentalService _rentalService;

        private int _totalVehicles;
        public int TotalVehicles
        {
            get => _totalVehicles;
            set => SetProperty(ref _totalVehicles, value);
        }

        private int _activeRentals;
        public int ActiveRentals
        {
            get => _activeRentals;
            set => SetProperty(ref _activeRentals, value);
        }

        private decimal _totalRevenue;
        public decimal TotalRevenue
        {
            get => _totalRevenue;
            set => SetProperty(ref _totalRevenue, value);
        }

        public ObservableCollection<string> RecentActivity { get; }

        public DashboardViewModel(IVehicleService vehicleService, IRentalService rentalService)
        {
            _vehicleService = vehicleService;
            _rentalService = rentalService;
            RecentActivity = new ObservableCollection<string>();
            Title = "Dashboard";

            LoadDashboardData();
        }

        private async void LoadDashboardData()
        {
            var vehicles = await _vehicleService.GetAllVehiclesAsync();
            var rentals = await _rentalService.GetAllRentalsAsync();

            TotalVehicles = vehicles.Count;
            ActiveRentals = rentals.Count(r => r.Status == "Active");
            TotalRevenue = rentals.Sum(r => r.TotalAmount);

            // Mock Activity
            RecentActivity.Add("10:30 AM | New Booking | John Doe | Toyota Camry");
            RecentActivity.Add("09:15 AM | Vehicle Return | Jane Smith | Ford Focus");
            RecentActivity.Add("08:45 AM | Maintenance | Tesla Model 3 | Battery Check");
        }
    }
}
