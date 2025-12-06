using CarRental.Application.Common.Models;
using CarRental.Application.DTOs;
using CarRental.Application.Interfaces;
using CarRental.Desktop.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CarRental.Desktop.ViewModels;

public class BookingManagementViewModel : ViewModelBase
{
    private readonly IBookingService _bookingService;
    private readonly IClientService _clientService;

    private ObservableCollection<ClientDto> _clients = new();
    private ObservableCollection<BookingDto> _bookings = new();
    private ClientDto? _selectedClient;
    private BookingDto? _selectedBooking;
    private bool _isLoading;

    public ObservableCollection<ClientDto> Clients
    {
        get => _clients;
        set => SetProperty(ref _clients, value);
    }

    public ObservableCollection<BookingDto> Bookings
    {
        get => _bookings;
        set => SetProperty(ref _bookings, value);
    }

    public ClientDto? SelectedClient
    {
        get => _selectedClient;
        set
        {
            if (SetProperty(ref _selectedClient, value) && value != null)
            {
                LoadBookingsForClientCommand.Execute(null);
            }
        }
    }

    public BookingDto? SelectedBooking
    {
        get => _selectedBooking;
        set => SetProperty(ref _selectedBooking, value);
    }

    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public ICommand LoadClientsCommand { get; }
    public ICommand LoadBookingsForClientCommand { get; }
    public ICommand CreateBookingCommand { get; }
    public ICommand CancelBookingCommand { get; }
    public ICommand GeneratePdfCommand { get; }

    public BookingManagementViewModel(
        IBookingService bookingService,
        IClientService clientService)
    {
        _bookingService = bookingService;
        _clientService = clientService;

        LoadClientsCommand = new RelayCommand(async (param) => await LoadClientsAsync());
        LoadBookingsForClientCommand = new RelayCommand(async (param) => await LoadBookingsForClientAsync());
        CreateBookingCommand = new RelayCommand(async (param) => await CreateBookingAsync());
        CancelBookingCommand = new RelayCommand(async (param) => await CancelBookingAsync());
        GeneratePdfCommand = new RelayCommand(async (param) => await GeneratePdfAsync());

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

                // Sélectionner le premier client par défaut
                if (Clients.Count > 0)
                {
                    SelectedClient = Clients.First();
                }
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadBookingsForClientAsync()
    {
        if (SelectedClient == null) return;

        IsLoading = true;
        try
        {
            var result = await _bookingService.GetClientBookingsAsync(SelectedClient.Id);

            if (result.IsSuccess && result.Value != null)
            {
                Bookings = new ObservableCollection<BookingDto>(result.Value);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task CreateBookingAsync()
    {
        if (SelectedClient == null) return;

        // TODO: Implémenter boîte de dialogue de création
        // var createDto = new CreateBookingDto(...);
        // var result = await _bookingService.CreateBookingAsync(createDto);

        // if (result.IsSuccess)
        // {
        //     await LoadBookingsForClientAsync();
        // }
    }

    private async Task CancelBookingAsync()
    {
        if (SelectedBooking == null || SelectedClient == null) return;

        // TODO: Implémenter boîte de dialogue de confirmation
        // var result = await _bookingService.CancelBookingAsync(SelectedBooking.Id, SelectedClient.Id);

        // if (result.IsSuccess && result.Value)
        // {
        //     await LoadBookingsForClientAsync();
        // }
    }

    private async Task GeneratePdfAsync()
    {
        if (SelectedBooking == null) return;

        var result = await _bookingService.GetBookingPdfAsync(SelectedBooking.Id);

        if (result.IsSuccess && result.Value != null)
        {
            // TODO: Sauvegarder le PDF
            // File.WriteAllBytes("reservation.pdf", result.Value);
        }
    }
}