using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using CarRental.Application.DTOs;
using CarRental.Desktop.Services;
 // Keeping for reference if needed, or remove if unused

namespace CarRental.Desktop.ViewModels
{
    public class VehicleTypesViewModel : ViewModelBase
    {
        private readonly IVehicleTypeService _vehicleTypeService;


        private ObservableCollection<VehicleTypeDto> _vehicleTypes;
        private VehicleTypeDto _selectedVehicleType;
        private bool _isEditing;
        private string _editTitle;

        // Form Properties
        private string _name;
        private string _description;
        private decimal _basePrice;

        public ObservableCollection<VehicleTypeDto> VehicleTypes
        {
            get => _vehicleTypes;
            set => SetProperty(ref _vehicleTypes, value);
        }

        public VehicleTypeDto SelectedVehicleType
        {
            get => _selectedVehicleType;
            set => SetProperty(ref _selectedVehicleType, value);
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

        public string Name { get => _name; set => SetProperty(ref _name, value); }
        public string Description { get => _description; set => SetProperty(ref _description, value); }
        public decimal BasePrice { get => _basePrice; set => SetProperty(ref _basePrice, value); }

        public ICommand LoadVehicleTypesCommand { get; }
        public ICommand AddVehicleTypeCommand { get; }
        public ICommand EditVehicleTypeCommand { get; }
        public ICommand DeleteVehicleTypeCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public VehicleTypesViewModel(IVehicleTypeService vehicleTypeService)
        {
            _vehicleTypeService = vehicleTypeService;
            _vehicleTypes = new ObservableCollection<VehicleTypeDto>();

            LoadVehicleTypesCommand = new RelayCommand(async _ => await LoadVehicleTypesAsync());
            AddVehicleTypeCommand = new RelayCommand(_ => StartEdit(null));
            EditVehicleTypeCommand = new RelayCommand(_ => StartEdit(SelectedVehicleType), _ => SelectedVehicleType != null);
            DeleteVehicleTypeCommand = new RelayCommand(async _ => await DeleteVehicleTypeAsync(), _ => SelectedVehicleType != null);
            SaveCommand = new RelayCommand(async _ => await SaveAsync());
            CancelCommand = new RelayCommand(_ => IsEditing = false);

            _ = LoadVehicleTypesAsync();
        }

        private async Task LoadVehicleTypesAsync()
        {
            var vehicleTypes = await _vehicleTypeService.GetAllVehicleTypesAsync();
            VehicleTypes = new ObservableCollection<VehicleTypeDto>(vehicleTypes);
        }

        private void StartEdit(VehicleTypeDto? vehicleType)
        {
            if (vehicleType == null)
            {
                EditTitle = "Add New Vehicle Type";
                Name = "";
                Description = "";
                BasePrice = 0;
                SelectedVehicleType = null;
            }
            else
            {
                EditTitle = "Edit Vehicle Type";
                Name = vehicleType.Name;
                Description = vehicleType.Description;
                BasePrice = vehicleType.BaseRate;
            }
            IsEditing = true;
        }

        private async Task SaveAsync()
        {
            if (SelectedVehicleType == null)
            {
                // Create New
                var newVehicleType = new CarRental.Application.DTOs.CreateVehicleTypeDto
                {
                    Name = Name,
                    Description = Description,
                    BaseRate = BasePrice
                };
                await _vehicleTypeService.AddVehicleTypeAsync(newVehicleType);
                await LoadVehicleTypesAsync();
            }
            else
            {
                // Update Existing
                SelectedVehicleType.Name = Name;
                SelectedVehicleType.Description = Description;
                SelectedVehicleType.BaseRate = BasePrice;

                await _vehicleTypeService.UpdateVehicleTypeAsync(SelectedVehicleType);
                
                var index = VehicleTypes.IndexOf(SelectedVehicleType);
                if (index >= 0) VehicleTypes[index] = SelectedVehicleType;
            }
            IsEditing = false;
        }

        private async Task DeleteVehicleTypeAsync()
        {
            if (SelectedVehicleType != null)
            {
                await _vehicleTypeService.DeleteVehicleTypeAsync(SelectedVehicleType.Id);
                VehicleTypes.Remove(SelectedVehicleType);
            }
        }
    }
}
