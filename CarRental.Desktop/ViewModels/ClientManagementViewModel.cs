using CarRental.Application.DTOs;
using CarRental.Desktop.Services;
using CarRental.Desktop.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CarRental.Desktop.ViewModels
{
    public class ClientManagementViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;

        private ObservableCollection<ClientDto> _clients = new();
        private ObservableCollection<ClientDto> _allClients = new();
        private ClientDto? _selectedClient;
        private string _searchText = string.Empty;

        public ClientManagementViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService;
            LoadClientsCommand = new AsyncRelayCommand(LoadClientsAsync);
            UpdateClientCommand = new AsyncRelayCommand(UpdateClientAsync, CanUpdateClient);
            DeleteClientCommand = new AsyncRelayCommand(DeleteClientAsync, CanDeleteClient);
            SearchCommand = new RelayCommand(_ => FilterClients());
            ClearSearchCommand = new RelayCommand(_ => ClearSearch());

            _ = LoadClientsAsync();
        }

        public ObservableCollection<ClientDto> Clients
        {
            get => _clients;
            set => SetProperty(ref _clients, value);
        }

        public ClientDto? SelectedClient
        {
            get => _selectedClient;
            set => SetProperty(ref _selectedClient, value);
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    FilterClients();
                }
            }
        }

        public ICommand LoadClientsCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand UpdateClientCommand { get; }
        public ICommand DeleteClientCommand { get; }
        public ICommand ClearSearchCommand { get; }

        private bool CanUpdateClient() => SelectedClient != null;
        private bool CanDeleteClient() => SelectedClient != null;

        private async Task LoadClientsAsync()
        {
            try
            {
                IsLoading = true;
                ClearError();

                // Données MOCK
                await Task.Delay(300);
                var mockClients = new[]
                {
                    new ClientDto { Id = 1, FullName = "Jean Dupont", Email = "jean@test.com", Phone = "0612345678", DriverLicense = "DL123", IsActive = true },
                    new ClientDto { Id = 2, FullName = "Marie Martin", Email = "marie@test.com", Phone = "0687654321", DriverLicense = "DL456", IsActive = true },
                    new ClientDto { Id = 3, FullName = "Pierre Bernard", Email = "pierre@test.com", Phone = "0698765432", DriverLicense = "DL789", IsActive = true }
                };

                _allClients = new ObservableCollection<ClientDto>(mockClients);
                Clients = new ObservableCollection<ClientDto>(_allClients);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                await _dialogService.ShowMessageAsync("Erreur", ErrorMessage);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void FilterClients()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Clients = new ObservableCollection<ClientDto>(_allClients);
                return;
            }

            var filtered = _allClients.Where(c =>
                c.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                c.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

            Clients = new ObservableCollection<ClientDto>(filtered);
        }

        private void ClearSearch()
        {
            SearchText = string.Empty;
        }

        private async Task UpdateClientAsync()
        {
            if (SelectedClient == null) return;
            await _dialogService.ShowMessageAsync("Info", $"Modification de {SelectedClient.FullName}");
        }

        private async Task DeleteClientAsync()
        {
            if (SelectedClient == null) return;

            var confirmed = await _dialogService.ShowConfirmationAsync(
                "Confirmation",
                $"Supprimer {SelectedClient.FullName} ?");

            if (confirmed)
            {
                _allClients.Remove(SelectedClient);
                Clients.Remove(SelectedClient);
                await _dialogService.ShowMessageAsync("Succès", "Client supprimé");
            }
        }
    }
}