using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CarRental.Desktop.Models;
using CarRental.Desktop.Services;

namespace CarRental.Desktop.ViewModels
{
    public class ClientsViewModel : ViewModelBase
    {
        private readonly IClientService _clientService;
        private ObservableCollection<Client> _clients;
        private Client _selectedClient;
        private bool _isEditing;
        private string _editTitle;

        // Form Properties
        private string _firstName;
        private string _lastName;
        private string _email;
        private string _phone;
        private string _licenseNumber;

        public ObservableCollection<Client> Clients
        {
            get => _clients;
            set => SetProperty(ref _clients, value);
        }

        public Client SelectedClient
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

        public string FirstName { get => _firstName; set => SetProperty(ref _firstName, value); }
        public string LastName { get => _lastName; set => SetProperty(ref _lastName, value); }
        public string Email { get => _email; set => SetProperty(ref _email, value); }
        public string Phone { get => _phone; set => SetProperty(ref _phone, value); }
        public string LicenseNumber { get => _licenseNumber; set => SetProperty(ref _licenseNumber, value); }

        public ICommand LoadClientsCommand { get; }
        public ICommand AddClientCommand { get; }
        public ICommand EditClientCommand { get; }
        public ICommand DeleteClientCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public ClientsViewModel(IClientService clientService)
        {
            _clientService = clientService;
            _clients = new ObservableCollection<Client>();

            LoadClientsCommand = new RelayCommand(async _ => await LoadClientsAsync());
            AddClientCommand = new RelayCommand(_ => StartEdit(null));
            EditClientCommand = new RelayCommand(_ => StartEdit(SelectedClient), _ => SelectedClient != null);
            DeleteClientCommand = new RelayCommand(async _ => await DeleteClientAsync(), _ => SelectedClient != null);
            SaveCommand = new RelayCommand(async _ => await SaveAsync());
            CancelCommand = new RelayCommand(_ => IsEditing = false);

            LoadClientsAsync();
        }

        private async Task LoadClientsAsync()
        {
            var clients = await _clientService.GetAllClientsAsync();
            Clients = new ObservableCollection<Client>(clients);
        }

        private void StartEdit(Client? client)
        {
            if (client == null)
            {
                EditTitle = "Add New Client";
                FirstName = "";
                LastName = "";
                Email = "";
                Phone = "";
                LicenseNumber = "";
                SelectedClient = null;
            }
            else
            {
                EditTitle = "Edit Client";
                FirstName = client.FirstName;
                LastName = client.LastName;
                Email = client.Email;
                Phone = client.Phone;
                LicenseNumber = client.LicenseNumber;
            }
            IsEditing = true;
        }

        private async Task SaveAsync()
        {
            if (SelectedClient == null)
            {
                var newClient = new Client
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Email = Email,
                    Phone = Phone,
                    LicenseNumber = LicenseNumber
                };
                await _clientService.AddClientAsync(newClient);
                Clients.Add(newClient);
            }
            else
            {
                SelectedClient.FirstName = FirstName;
                SelectedClient.LastName = LastName;
                SelectedClient.Email = Email;
                SelectedClient.Phone = Phone;
                SelectedClient.LicenseNumber = LicenseNumber;

                var index = Clients.IndexOf(SelectedClient);
                Clients[index] = SelectedClient;
            }
            IsEditing = false;
        }

        private async Task DeleteClientAsync()
        {
            if (SelectedClient != null)
            {
                Clients.Remove(SelectedClient);
                await _clientService.DeleteClientAsync(SelectedClient.Id);
            }
        }
    }
}
