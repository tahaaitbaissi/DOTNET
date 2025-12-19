using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CarRental.Application.DTOs;
using CarRental.Desktop.Services;

namespace CarRental.Desktop.ViewModels
{
    public class RentalsViewModel : ViewModelBase
    {
        private readonly IRentalService _rentalService;
        private readonly IClientService _clientService;
        private readonly IVehicleService _vehicleService;
        private readonly IDialogService _dialogService;

        private ObservableCollection<BookingDto> _rentals;
        private BookingDto _selectedRental;
        private bool _isAdding;
        
        // Form Data
        private ObservableCollection<ClientDto> _clients;
        private ObservableCollection<VehicleDto> _vehicles;
        private ClientDto _selectedClient;
        private VehicleDto _selectedVehicle;
        private DateTime _newStartDate = DateTime.Today;
        private DateTime _newEndDate = DateTime.Today.AddDays(1);
        private string _newPickUpLocation = "";
        private string _newDropOffLocation = "";
        private string _newNotes = "";

        public ObservableCollection<BookingDto> Rentals
        {
            get => _rentals;
            set => SetProperty(ref _rentals, value);
        }

        public BookingDto SelectedRental
        {
            get => _selectedRental;
            set => SetProperty(ref _selectedRental, value);
        }

        public bool IsAdding
        {
            get => _isAdding;
            set => SetProperty(ref _isAdding, value);
        }

        public ObservableCollection<ClientDto> Clients
        {
            get => _clients;
            set => SetProperty(ref _clients, value);
        }

        public ObservableCollection<VehicleDto> Vehicles
        {
            get => _vehicles;
            set => SetProperty(ref _vehicles, value);
        }

        public ClientDto SelectedClient
        {
            get => _selectedClient;
            set => SetProperty(ref _selectedClient, value);
        }

        public VehicleDto SelectedVehicle
        {
            get => _selectedVehicle;
            set => SetProperty(ref _selectedVehicle, value);
        }

        public DateTime NewStartDate
        {
            get => _newStartDate;
            set => SetProperty(ref _newStartDate, value);
        }

        public DateTime NewEndDate
        {
            get => _newEndDate;
            set => SetProperty(ref _newEndDate, value);
        }

        public string NewPickUpLocation
        {
            get => _newPickUpLocation;
            set => SetProperty(ref _newPickUpLocation, value);
        }

        public string NewDropOffLocation
        {
            get => _newDropOffLocation;
            set => SetProperty(ref _newDropOffLocation, value);
        }
        
        public string NewNotes
        {
            get => _newNotes;
            set => SetProperty(ref _newNotes, value);
        }

        public ICommand LoadRentalsCommand { get; }
        public ICommand CompleteRentalCommand { get; }
        public ICommand CancelRentalCommand { get; }
        public ICommand AddRentalCommand { get; }
        public ICommand EditRentalCommand { get; }
        public ICommand SaveRentalCommand { get; }
        public ICommand CancelAddCommand { get; }
        public ICommand DeleteRentalCommand { get; }
        
        private long _editingRentalId = 0; // 0 means creating new

        public RentalsViewModel(
            IRentalService rentalService, 
            IClientService clientService, 
            IVehicleService vehicleService, 
            IDialogService dialogService)
        {
            _rentalService = rentalService;
            _clientService = clientService;
            _vehicleService = vehicleService;
            _dialogService = dialogService;

            _rentals = new ObservableCollection<BookingDto>();
            _clients = new ObservableCollection<ClientDto>();
            _vehicles = new ObservableCollection<VehicleDto>();

            LoadRentalsCommand = new RelayCommand(async _ => await LoadRentalsAsync());
            CompleteRentalCommand = new RelayCommand(async _ => await UpdateStatusAsync("Completed"), _ => SelectedRental != null && SelectedRental.Status == "Active");
            CancelRentalCommand = new RelayCommand(async _ => await UpdateStatusAsync("Cancelled"), _ => SelectedRental != null && SelectedRental.Status == "Active");
            
            AddRentalCommand = new RelayCommand(_ => StartAdding());
            EditRentalCommand = new RelayCommand(_ => StartEditing(), _ => SelectedRental != null);
            SaveRentalCommand = new RelayCommand(async _ => await SaveRentalAsync());
            CancelAddCommand = new RelayCommand(_ => { IsAdding = false; _editingRentalId = 0; });
            DeleteRentalCommand = new RelayCommand(async _ => await DeleteRentalAsync(), _ => SelectedRental != null);

            _ = LoadRentalsAsync();
        }

        private async Task LoadRentalsAsync()
        {
            if (!SessionManager.IsLoggedIn) return;

            IsLoading = true;
            try 
            {
                var rentals = await _rentalService.GetAllRentalsAsync();
                Rentals = new ObservableCollection<BookingDto>(rentals);
            }
            finally 
            {
                IsLoading = false;
            }
        }

        private async void StartAdding()
        {
            _editingRentalId = 0;
            // Reset form
            NewStartDate = DateTime.Today;
            NewEndDate = DateTime.Today.AddDays(1);
            NewPickUpLocation = "Agency";
            NewDropOffLocation = "Agency";
            NewNotes = "";
            SelectedClient = null; // Will trigger "required" check on save
            SelectedVehicle = null;

            await LoadFormDataAsync();
            IsAdding = true;
        }

        private async void StartEditing()
        {
            if (SelectedRental == null) return;
            
            _editingRentalId = SelectedRental.Id;
            
            await LoadFormDataAsync();

            // Pre-fill form
            NewStartDate = SelectedRental.StartDate;
            NewEndDate = SelectedRental.EndDate;
            NewPickUpLocation = SelectedRental.PickUpLocation;
            NewDropOffLocation = SelectedRental.DropOffLocation;
            NewNotes = SelectedRental.Notes ?? "";
            
            // Should select correct client/vehicle in combobox
            // Need to match by ID
            // Assuming LoadFormDataAsync populates lists first
            
            if (Clients != null)
                SelectedClient = System.Linq.Enumerable.FirstOrDefault(Clients, c => c.Id == SelectedRental.ClientId);
            
            if (Vehicles != null)
                SelectedVehicle = System.Linq.Enumerable.FirstOrDefault(Vehicles, v => v.Id == SelectedRental.VehicleId);

            IsAdding = true;
        }

        private async Task LoadFormDataAsync()
        {
            IsLoading = true;
            try
            {
                if (Clients == null || Clients.Count == 0)
                {
                    var clients = await _clientService.GetAllClientsAsync();
                    Clients = new ObservableCollection<ClientDto>(clients);
                }

                if (Vehicles == null || Vehicles.Count == 0)
                {
                    var vehicles = await _vehicleService.GetAllVehiclesAsync();
                    Vehicles = new ObservableCollection<VehicleDto>(vehicles);
                }
            }
            catch (Exception ex)
            {
                 await _dialogService.ShowErrorAsync("Error", "Failed to load data: " + ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task SaveRentalAsync()
        {
            if (SelectedClient == null || SelectedVehicle == null)
            {
                await _dialogService.ShowWarningAsync("Validation", "Please select a client and a vehicle.");
                return;
            }
            
            IsLoading = true;
            try
            {
                if (_editingRentalId == 0) // Create
                {
                    var dto = new CreateBookingDto(
                        SelectedClient.Id, 
                        SelectedVehicle.Id, 
                        NewStartDate, 
                        NewEndDate, 
                        NewPickUpLocation, 
                        NewDropOffLocation, 
                        NewNotes
                    );
                    await _rentalService.CreateRentalAsync(dto);
                    await _dialogService.ShowSuccessAsync("Success", "Rental created successfully.");
                }
                else // Update
                {
                     var dto = new UpdateBookingDto(
                        SelectedVehicle.Id, 
                        NewStartDate, 
                        NewEndDate, 
                        NewPickUpLocation, 
                        NewDropOffLocation, 
                        NewNotes
                    );
                    await _rentalService.UpdateRentalAsync(_editingRentalId, dto);
                     await _dialogService.ShowSuccessAsync("Success", "Rental updated successfully.");
                }

                IsAdding = false;
                _editingRentalId = 0;
                await LoadRentalsAsync();
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("Error", "Operation failed: " + ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task DeleteRentalAsync()
        {
            if (SelectedRental == null) return;

            var confirm = await _dialogService.ShowConfirmationAsync("Confirm Delete", $"Are you sure you want to delete rental #{SelectedRental.Id}?");
            if (!confirm) return;

            IsLoading = true;
            try
            {
                await _rentalService.DeleteRentalAsync(SelectedRental.Id);
                await LoadRentalsAsync();
                await _dialogService.ShowSuccessAsync("Success", "Rental deleted.");
            }
            catch (Exception ex)
            {
                await _dialogService.ShowErrorAsync("Error", "Failed to delete: " + ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task UpdateStatusAsync(string newStatus)
        {
            if (SelectedRental != null)
            {
                try
                {
                    SelectedRental.Status = newStatus;
                    await _rentalService.UpdateRentalStatusAsync(SelectedRental.Id, newStatus);
                    
                    var index = Rentals.IndexOf(SelectedRental);
                    if (index >= 0)
                    {
                        Rentals[index] = SelectedRental;
                    }
                     await _dialogService.ShowSuccessAsync("Success", $"Rental status updated to {newStatus}");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowErrorAsync("Error", "Failed to update status: " + ex.Message);
                }
            }
        }
    }
}
