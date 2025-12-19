using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using CarRental.Desktop.Services;

namespace CarRental.Desktop.ViewModels
{
    public class AvailabilityViewModel : ViewModelBase
    {
        private readonly IVehicleService _vehicleService;
        private ObservableCollection<CarRental.Application.DTOs.VehicleDto> _availableVehicles;
        private DateTime _startDate = DateTime.Today;
        private DateTime _endDate = DateTime.Today.AddDays(1);

        public ObservableCollection<CarRental.Application.DTOs.VehicleDto> AvailableVehicles
        {
            get => _availableVehicles;
            set => SetProperty(ref _availableVehicles, value);
        }

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                if (SetProperty(ref _startDate, value))
                {
                    CheckAvailability();
                }
            }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                if (SetProperty(ref _endDate, value))
                {
                    CheckAvailability();
                }
            }
        }

        public ICommand CheckAvailabilityCommand { get; }

        public AvailabilityViewModel(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
            _availableVehicles = new ObservableCollection<CarRental.Application.DTOs.VehicleDto>();
            CheckAvailabilityCommand = new RelayCommand(_ => CheckAvailability());
            
            Title = "Check Availability";
            
            // Initial Load
            CheckAvailability();
        }

        private async void CheckAvailability()
        {
            IsLoading = true;
            
            // In a real app, we'd pass dates to the API
            // For now, let's just filter the list from GetAllVehiclesAsync
            var vehicles = await _vehicleService.GetAvailableVehiclesAsync(StartDate, EndDate);
            
            AvailableVehicles = new ObservableCollection<CarRental.Application.DTOs.VehicleDto>(vehicles);
            
            IsLoading = false;
        }
    }
}
