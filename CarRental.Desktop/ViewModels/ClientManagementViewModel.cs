using CarRental.Application.Common.Models;
using CarRental.Application.DTOs;
using CarRental.Application.Interfaces;
using CarRental.Desktop.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace CarRental.Desktop.ViewModels;

public class ClientManagementViewModel : ViewModelBase
{
    private readonly IClientService _clientService;

    private ObservableCollection<ClientDto> _clients = new();
    private ClientDto? _selectedClient;
    private string _searchText = string.Empty;
    private bool _isLoading;

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

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    // Propriétés pour l'affichage détaillé
    public string SelectedClientName => SelectedClient?.FullName ?? "Aucun client sélectionné";
    public string SelectedClientEmail => SelectedClient?.Email ?? "";
    public string SelectedClientPhone => SelectedClient?.Phone ?? "";
    public string SelectedClientLicense => SelectedClient?.DriverLicense ?? "";

    public ICommand LoadClientsCommand { get; }
    public ICommand UpdateClientCommand { get; }
    public ICommand DeleteClientCommand { get; }
    public ICommand ClearSearchCommand { get; }
    public ICommand ExportClientsCommand { get; }

    public ClientManagementViewModel(IClientService clientService)
    {
        _clientService = clientService;

        LoadClientsCommand = new RelayCommand(async (param) => await LoadClientsAsync());
        UpdateClientCommand = new RelayCommand(async (param) => await UpdateClientAsync());
        DeleteClientCommand = new RelayCommand(async (param) => await DeleteClientAsync());
        ClearSearchCommand = new RelayCommand((param) => ClearSearch());
        ExportClientsCommand = new RelayCommand(async (param) => await ExportClientsAsync());

        LoadClientsCommand.Execute(null);
    }

    private async Task LoadClientsAsync()
    {
        IsLoading = true;
        try
        {
            var result = await _clientService.GetAllClientsAsync();

            if (result.IsSuccess && result.Value != null)
            {
                Clients = new ObservableCollection<ClientDto>(result.Value);

                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    FilterClients();
                }
            }
            else
            {
                Console.WriteLine($"Erreur: {result.Error}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
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
            LoadClientsCommand.Execute(null);
            return;
        }

        var allClients = _clients.ToList();
        var filtered = allClients.Where(c =>
            c.FullName.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            c.Email.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
            c.Phone.Contains(SearchText) ||
            c.DriverLicense.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

        Clients = new ObservableCollection<ClientDto>(filtered);
    }

    private void ClearSearch()
    {
        SearchText = string.Empty;
        LoadClientsCommand.Execute(null);
    }

    private async Task UpdateClientAsync()
    {
        if (SelectedClient == null) return;

        // TODO: Implémenter boîte de dialogue de modification
        // var updateDto = new UpdateClientDto 
        // {
        //     // Mapper les propriétés
        //     FullName = SelectedClient.FullName,
        //     Email = SelectedClient.Email,
        //     Phone = SelectedClient.Phone,
        //     Address = SelectedClient.Address,
        //     DriverLicense = SelectedClient.DriverLicense,
        //     LicenseExpiry = SelectedClient.LicenseExpiry
        // };

        // var result = await _clientService.UpdateClientAsync(SelectedClient.Id, updateDto);

        // if (result.IsSuccess)
        // {
        //     await LoadClientsAsync();
        // }
    }

    private async Task DeleteClientAsync()
    {
        if (SelectedClient == null) return;

        // TODO: Implémenter boîte de dialogue de confirmation
        // var result = await _clientService.DeleteClientAsync(SelectedClient.Id);

        // if (result.IsSuccess && result.Value)
        // {
        //     await LoadClientsAsync();
        // }
    }

    private async Task ExportClientsAsync()
    {
        // TODO: Utiliser IExportService.ExportClientsAsync()
        Console.WriteLine("Export des clients...");
    }
}