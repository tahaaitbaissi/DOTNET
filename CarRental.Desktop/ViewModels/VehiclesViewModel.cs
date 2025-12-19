using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CarRental.Application.DTOs;
using CarRental.Desktop.Services;
using CarRental.Core.Enums;
using System;

namespace CarRental.Desktop.ViewModels
{
    public class VehiclesViewModel : ViewModelBase
    {
        private readonly IVehicleService _vehicleService;
        private ObservableCollection<VehicleDto> _vehicles;
        private VehicleDto _selectedVehicle;
        private bool _isEditing;
        private string _editTitle;

        // Form Properties
        private string _make;
        private string _model;
        private int _year;
        private string _licensePlate;
        private decimal _dailyRate;
        private string _status; // Simple string for now, or could map to Enum
        private string _imagePath;
        private bool _isGridView;

        public bool IsGridView
        {
            get => _isGridView;
            set => SetProperty(ref _isGridView, value);
        }

        public ObservableCollection<VehicleDto> Vehicles
        {
            get => _vehicles;
            set => SetProperty(ref _vehicles, value);
        }

        public VehicleDto SelectedVehicle
        {
            get => _selectedVehicle;
            set => SetProperty(ref _selectedVehicle, value);
        }

        public bool IsEditing
        {
            get => _isEditing;
            set => SetProperty(ref _isEditing, value);
        }

        public string EditTitle
        {
            get => _editTitle;
            set => SetProperty(ref _editTitle, value);
        }

        // Form Bindings
        public string Make { get => _make; set => SetProperty(ref _make, value); }
        public string Model { get => _model; set => SetProperty(ref _model, value); }
        public int Year { get => _year; set => SetProperty(ref _year, value); }
        public string LicensePlate { get => _licensePlate; set => SetProperty(ref _licensePlate, value); }
        public decimal DailyRate { get => _dailyRate; set => SetProperty(ref _dailyRate, value); }
        public string Status { get => _status; set => SetProperty(ref _status, value); }
        public string ImagePath { get => _imagePath; set => SetProperty(ref _imagePath, value); }

        public ICommand LoadVehiclesCommand { get; }
        public ICommand AddVehicleCommand { get; }
        public ICommand EditVehicleCommand { get; }
        public ICommand DeleteVehicleCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand BrowseImageCommand { get; }

        public VehiclesViewModel(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
            _vehicles = new ObservableCollection<VehicleDto>();

            LoadVehiclesCommand = new RelayCommand(async _ => await LoadVehiclesAsync());
            AddVehicleCommand = new RelayCommand(_ => StartEdit(null));
            EditVehicleCommand = new RelayCommand(_ => StartEdit(SelectedVehicle), _ => SelectedVehicle != null);
            DeleteVehicleCommand = new RelayCommand(async _ => await DeleteVehicleAsync(), _ => SelectedVehicle != null);
            SaveCommand = new RelayCommand(async _ => await SaveAsync());
            CancelCommand = new RelayCommand(_ => IsEditing = false);
            BrowseImageCommand = new RelayCommand(ExecuteBrowseImage);
            
            Title = "Vehicles";

            _ = LoadVehiclesAsync();
        }

        private async Task LoadVehiclesAsync()
        {
            if (!SessionManager.IsLoggedIn) return;

            var vehicles = await _vehicleService.GetAllVehiclesAsync();
            Vehicles = new ObservableCollection<VehicleDto>(vehicles);
        }

        private void StartEdit(VehicleDto? vehicle)
        {
            if (vehicle == null)
            {
                // Add New
                EditTitle = "Add New Vehicle";
                Make = "";
                Model = "";
                Year = DateTime.Now.Year;
                LicensePlate = "";
                DailyRate = 0;
                Status = "Available";
                ImagePath = "";
                SelectedVehicle = null; // Ensure we are in "Add" mode
            }
            else
            {
                // Edit Existing
                EditTitle = "Edit Vehicle";
                Make = vehicle.Make;
                Model = vehicle.Model;
                Year = vehicle.Year;
                LicensePlate = vehicle.LicensePlate;
                DailyRate = vehicle.DailyRate;
                Status = vehicle.Status;
                // VehicleDto uses ImageUrls List, just use first one or empty
                ImagePath = (vehicle.ImageUrls != null && vehicle.ImageUrls.Count > 0) ? vehicle.ImageUrls[0] : "";
            }
            IsEditing = true;
        }

        private async Task SaveAsync()
        {
            if (SelectedVehicle == null)
            {
                // Create New
                // Map string status to Enum
                Enum.TryParse(Status, true, out VehicleStatus vehicleStatus);

                var newVehicle = new CreateVehicleDto
                {
                    Make = Make,
                    Model = Model,
                    Year = Year,
                    LicensePlate = LicensePlate,
                    PricePerDay = DailyRate,
                    Status = vehicleStatus,
                    VIN = Guid.NewGuid().ToString().Substring(0, 17) // Dummy VIN for now if not required by UI
                };
                
                // Note: Image upload not handled in CreateVehicleDto yet
                
                await _vehicleService.AddVehicleAsync(newVehicle);
                await LoadVehiclesAsync(); // Refresh list to get ID and everything back
            }
            else
            {
                // Update Existing - Not implemented in IVehicleService yet for this demo
                // But typically would call UpdateVehicleAsync
            }
            IsEditing = false;
        }

        private void ExecuteBrowseImage(object? obj)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image files (*.png;*.jpeg;*.jpg)|*.png;*.jpeg;*.jpg|All files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                ImagePath = openFileDialog.FileName;
            }
        }

        private async Task DeleteVehicleAsync()
        {
            if (SelectedVehicle != null)
            {
                // API call to delete needed
                // await _vehicleService.DeleteVehicleAsync(SelectedVehicle.Id);
                Vehicles.Remove(SelectedVehicle);
            }
        }
    }
}
