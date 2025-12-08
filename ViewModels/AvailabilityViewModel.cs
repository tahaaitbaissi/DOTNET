using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CarRental.Desktop.Models;
using CarRental.Desktop.Services;

namespace CarRental.Desktop.ViewModels
{
    public class AvailabilityViewModel : ViewModelBase
    {
        private readonly IVehicleService _vehicleService;
        private ObservableCollection<Vehicle> _availableVehicles;

        public ObservableCollection<Vehicle> AvailableVehicles
        {
            get => _availableVehicles;
            set => SetProperty(ref _availableVehicles, value);
        }

        public ICommand RefreshCommand { get; }

        public AvailabilityViewModel(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
            _availableVehicles = new ObservableCollection<Vehicle>();
            RefreshCommand = new RelayCommand(async _ => await LoadAvailableVehiclesAsync());
            LoadAvailableVehiclesAsync();
        }

        private async Task LoadAvailableVehiclesAsync()
        {
            var vehicles = await _vehicleService.GetAllVehiclesAsync();
            var available = vehicles.Where(v => v.Status == "Available").ToList();
            AvailableVehicles = new ObservableCollection<Vehicle>(available);
        }
    }
}
