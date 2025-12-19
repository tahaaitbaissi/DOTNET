using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

using CarRental.Desktop.Services;
using CarRental.Application.DTOs;

namespace CarRental.Desktop.ViewModels
{
    public class ClientsViewModel : ViewModelBase
    {
        private readonly IClientService _clientService;
        private ObservableCollection<ClientDto> _clients;
        private ClientDto _selectedClient;
        private bool _isEditing;
        private string _editTitle;

        // Form Properties
        private string _fullName;
        private string _email;
        private string _phone;
        private string _licenseNumber;
        private string _address;
        private DateTime _licenseExpiry = DateTime.Today.AddYears(1);

        public ObservableCollection<ClientDto> Clients
        {
            get => _clients;
            set => SetProperty(ref _clients, value);
        }

        public ClientDto SelectedClient
        {
            get => _selectedClient;
            set => SetProperty(ref _selectedClient, value);
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

        public string FullName { get => _fullName; set => SetProperty(ref _fullName, value); }
        public string Email { get => _email; set => SetProperty(ref _email, value); }
        // Email usually read-only in update, or handled via identity
        public string Phone { get => _phone; set => SetProperty(ref _phone, value); }
        public string LicenseNumber { get => _licenseNumber; set => SetProperty(ref _licenseNumber, value); }
        public string Address { get => _address; set => SetProperty(ref _address, value); }
        public DateTime LicenseExpiry { get => _licenseExpiry; set => SetProperty(ref _licenseExpiry, value); }

        public ICommand LoadClientsCommand { get; }
        public ICommand AddClientCommand { get; }
        public ICommand EditClientCommand { get; }
        public ICommand DeleteClientCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public ClientsViewModel(IClientService clientService)
        {
            _clientService = clientService;
            _clients = new ObservableCollection<ClientDto>();

            LoadClientsCommand = new RelayCommand(async _ => await LoadClientsAsync());
            // Add Client enabled
            AddClientCommand = new RelayCommand(_ => StartEdit(null)); 
            EditClientCommand = new RelayCommand(_ => StartEdit(SelectedClient), _ => SelectedClient != null);
            DeleteClientCommand = new RelayCommand(async _ => await DeleteClientAsync(), _ => SelectedClient != null);
            SaveCommand = new RelayCommand(async _ => await SaveAsync());
            CancelCommand = new RelayCommand(_ => IsEditing = false);

            Title = "Clients";

            _ = LoadClientsAsync();
        }

        private async Task LoadClientsAsync()
        {
            if (!SessionManager.IsLoggedIn) return;
            var clients = await _clientService.GetAllClientsAsync();
            Clients = new ObservableCollection<ClientDto>(clients);
        }

        private void StartEdit(ClientDto? client)
        {
            if (client == null)
            {
                // Add New
                EditTitle = "Add New Client";
                FullName = "";
                Email = "";
                Phone = "";
                LicenseNumber = "";
                Address = "";
                LicenseExpiry = DateTime.Today.AddYears(1);
                SelectedClient = null;
            }
            else
            {
                EditTitle = "Edit Client";
                FullName = client.FullName;
                Email = client.Email;
                Phone = client.Phone;
                LicenseNumber = client.DriverLicense;
                Address = client.Address;
                LicenseExpiry = client.LicenseExpiry;
            }
            IsEditing = true;
        }

        private async Task SaveAsync()
        {
            if (SelectedClient != null)
            {
                // Update Existing
                var updateDto = new CarRental.Application.DTOs.UpdateClientDto
                {
                    FullName = FullName,
                    Phone = Phone,
                    Address = Address,
                    DriverLicense = LicenseNumber,
                    LicenseExpiry = LicenseExpiry
                };

                await _clientService.UpdateClientAsync(SelectedClient.Id, updateDto);
            }
            else
            {
                 // Create New
                var createDto = new CarRental.Application.DTOs.CreateClientDto(
                    FullName,
                    Email,
                    Phone,
                    Address,
                    LicenseNumber,
                    LicenseExpiry
                );
                
                await _clientService.CreateClientAsync(createDto);
            }

            IsEditing = false;
            // Refresh list
            await LoadClientsAsync();
        }

        private async Task DeleteClientAsync()
        {
            if (SelectedClient != null)
            {
                await _clientService.DeleteClientAsync(SelectedClient.Id);
                Clients.Remove(SelectedClient);
            }
        }
    }
}
