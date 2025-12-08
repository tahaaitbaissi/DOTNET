using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CarRental.Desktop.Models;
using CarRental.Desktop.Services;

namespace CarRental.Desktop.ViewModels
{
    public class VehiclesViewModel : ViewModelBase
    {
        private readonly IVehicleService _vehicleService;
        private ObservableCollection<Vehicle> _vehicles;
        private Vehicle _selectedVehicle;
        private bool _isEditing;
        private string _editTitle;

        // Form Properties
        private string _make;
        private string _model;
        private int _year;
        private string _licensePlate;
        private decimal _dailyRate;
        private string _status;
        private string _imagePath;
        private bool _isGridView;

        public bool IsGridView
        {
            get => _isGridView;
            set => SetProperty(ref _isGridView, value);
        }

        public ObservableCollection<Vehicle> Vehicles
        {
            get => _vehicles;
            set => SetProperty(ref _vehicles, value);
        }

        public Vehicle SelectedVehicle
        {
            get => _selectedVehicle;
            set
            {
                if (SetProperty(ref _selectedVehicle, value) && value != null)
                {
                    // Optional: Auto-open edit on selection? No, let's use a button.
                }
            }
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
            _vehicles = new ObservableCollection<Vehicle>();

            LoadVehiclesCommand = new RelayCommand(async _ => await LoadVehiclesAsync());
            AddVehicleCommand = new RelayCommand(_ => StartEdit(null));
            EditVehicleCommand = new RelayCommand(_ => StartEdit(SelectedVehicle), _ => SelectedVehicle != null);
            DeleteVehicleCommand = new RelayCommand(async _ => await DeleteVehicleAsync(), _ => SelectedVehicle != null);
            SaveCommand = new RelayCommand(async _ => await SaveAsync());
            CancelCommand = new RelayCommand(_ => IsEditing = false);
            BrowseImageCommand = new RelayCommand(ExecuteBrowseImage);
            
            Title = "Vehicles";

            LoadVehiclesAsync();
        }

        private async Task LoadVehiclesAsync()
        {
            var vehicles = await _vehicleService.GetAllVehiclesAsync();
            Vehicles = new ObservableCollection<Vehicle>(vehicles);
        }

        private void StartEdit(Vehicle? vehicle)
        {
            if (vehicle == null)
            {
                // Add New
                EditTitle = "Add New Vehicle";
                Make = "";
                Model = "";
                Year = 2025;
                LicensePlate = "";
                DailyRate = 0;
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
                ImagePath = vehicle.ImagePath;
            }
            IsEditing = true;
        }

        private async Task SaveAsync()
        {
            if (SelectedVehicle == null)
            {
                // Create New
                var newVehicle = new Vehicle
                {
                    Make = Make,
                    Model = Model,
                    Year = Year,
                    LicensePlate = LicensePlate,
                    DailyRate = DailyRate,
                    Status = Status,
                    ImagePath = ImagePath
                };
                await _vehicleService.AddVehicleAsync(newVehicle);
                Vehicles.Add(newVehicle);
            }
            else
            {
                // Update Existing
                SelectedVehicle.Make = Make;
                SelectedVehicle.Model = Model;
                SelectedVehicle.Year = Year;
                SelectedVehicle.LicensePlate = LicensePlate;
                SelectedVehicle.DailyRate = DailyRate;
                SelectedVehicle.Status = Status;
                SelectedVehicle.ImagePath = ImagePath;
                
                // Trigger UI update (hacky, better to implement INPC on Model or wrap in ViewModel)
                var index = Vehicles.IndexOf(SelectedVehicle);
                Vehicles[index] = SelectedVehicle; 
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
                Vehicles.Remove(SelectedVehicle);
                // await _vehicleService.DeleteVehicleAsync(SelectedVehicle.Id);
            }
        }
    }
}
