using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CarRental.Desktop.Models;
using CarRental.Desktop.Services;

namespace CarRental.Desktop.ViewModels
{
    public class VehicleTypesViewModel : ViewModelBase
    {
        private readonly IVehicleTypeService _vehicleTypeService;

        private ObservableCollection<VehicleType> _vehicleTypes;
        public ObservableCollection<VehicleType> VehicleTypes
        {
            get => _vehicleTypes;
            set => SetProperty(ref _vehicleTypes, value);
        }

        private VehicleType? _selectedVehicleType;
        public VehicleType? SelectedVehicleType
        {
            get => _selectedVehicleType;
            set
            {
                if (SetProperty(ref _selectedVehicleType, value))
                {
                    // Optional: Update command cancel/execute status
                }
            }
        }

        // Edit State
        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set => SetProperty(ref _isEditing, value);
        }

        private string _editTitle = "Add Vehicle Type";
        public string EditTitle
        {
            get => _editTitle;
            set => SetProperty(ref _editTitle, value);
        }

        // Form Fields
        private string _name = string.Empty;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _description = string.Empty;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private decimal _basePrice;
        public decimal BasePrice
        {
            get => _basePrice;
            set => SetProperty(ref _basePrice, value);
        }

        private int _editingId; // 0 for new

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public VehicleTypesViewModel(IVehicleTypeService vehicleTypeService)
        {
            Title = "Vehicle Types";
            _vehicleTypeService = vehicleTypeService;
            VehicleTypes = new ObservableCollection<VehicleType>();

            AddCommand = new RelayCommand(ExecuteAdd);
            EditCommand = new RelayCommand(ExecuteEdit, o => SelectedVehicleType != null);
            DeleteCommand = new RelayCommand(ExecuteDelete, o => SelectedVehicleType != null);
            SaveCommand = new RelayCommand(ExecuteSave);
            CancelCommand = new RelayCommand(ExecuteCancel);

            LoadData();
        }

        private async void LoadData()
        {
            var data = await _vehicleTypeService.GetAllVehicleTypesAsync();
            VehicleTypes = new ObservableCollection<VehicleType>(data);
        }

        private void ExecuteAdd(object? obj)
        {
            _editingId = 0;
            Name = "";
            Description = "";
            BasePrice = 0;
            EditTitle = "Add Vehicle Type";
            IsEditing = true;
        }

        private void ExecuteEdit(object? obj)
        {
            if (SelectedVehicleType == null) return;

            _editingId = SelectedVehicleType.Id;
            Name = SelectedVehicleType.Name;
            Description = SelectedVehicleType.Description;
            BasePrice = SelectedVehicleType.BasePrice;
            EditTitle = "Edit Vehicle Type";
            IsEditing = true;
        }

        private async void ExecuteDelete(object? obj)
        {
            if (SelectedVehicleType == null) return;

            await _vehicleTypeService.DeleteVehicleTypeAsync(SelectedVehicleType.Id);
            LoadData();
        }

        private async void ExecuteSave(object? obj)
        {
            var vehicleType = new VehicleType
            {
                Id = _editingId,
                Name = Name,
                Description = Description,
                BasePrice = BasePrice
            };

            if (_editingId == 0)
            {
                await _vehicleTypeService.AddVehicleTypeAsync(vehicleType);
            }
            else
            {
                await _vehicleTypeService.UpdateVehicleTypeAsync(vehicleType);
            }

            IsEditing = false;
            LoadData();
        }

        private void ExecuteCancel(object? obj)
        {
            IsEditing = false;
        }
    }
}
