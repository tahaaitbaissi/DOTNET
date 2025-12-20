using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using CarRental.Desktop.Services;

namespace CarRental.Desktop.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private readonly IDashboardService _dashboardService;

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

        public DashboardViewModel(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
            RecentActivity = new ObservableCollection<string>();
            Title = "Dashboard";

            LoadDashboardData();
        }

        private async void LoadDashboardData()
        {
            IsLoading = true;
            
            if (!SessionManager.IsLoggedIn)
            {
                IsLoading = false;
                return;
            }
            
            var result = await _dashboardService.GetDashboardDataAsync();

            if (result.IsSuccess && result.Value != null)
            {
                var data = result.Value;
                TotalVehicles = data.TotalVehicles;
                // Active rentals might be mapped to one of the fields, checking DTO:
                // DTO has ActiveBookings and RentedVehicles. Let's use RentedVehicles for ActiveRentals
                ActiveRentals = data.RentedVehicles;
                TotalRevenue = data.TotalRevenue;

                // Populate Recent Activity from RecentBookings
                RecentActivity.Clear();
                if (data.RecentBookings != null)
                {
                    foreach (var booking in data.RecentBookings)
                    {
                        // Using string formatting to match previous style
                        RecentActivity.Add($"{booking.StartDate:t} | New Booking | {booking.ClientName} | {booking.VehicleName}");
                    }
                }
            }
            else
            {
                // Handle error (maybe show message)
                System.Diagnostics.Debug.WriteLine($"Dashboard load failed: {result.Error}");
            }
            
            IsLoading = false;
        }
    }
}
