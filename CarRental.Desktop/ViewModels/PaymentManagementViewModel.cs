using CarRental.Application.Common.Models;
using CarRental.Application.DTOs;
using CarRental.Application.Interfaces;
using CarRental.Desktop.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CarRental.Desktop.ViewModels;

public class PaymentManagementViewModel : ViewModelBase
{
    private readonly IPaymentService _paymentService;
    private readonly IBookingService _bookingService;

    private ObservableCollection<PaymentDto> _payments = new();
    private ObservableCollection<BookingDto> _unpaidBookings = new();
    private PaymentDto? _selectedPayment;
    private BookingDto? _selectedBooking;
    private bool _isLoading;

    public ObservableCollection<PaymentDto> Payments
    {
        get => _payments;
        set => SetProperty(ref _payments, value);
    }

    public ObservableCollection<BookingDto> UnpaidBookings
    {
        get => _unpaidBookings;
        set => SetProperty(ref _unpaidBookings, value);
    }

    public PaymentDto? SelectedPayment
    {
        get => _selectedPayment;
        set => SetProperty(ref _selectedPayment, value);
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

    public ICommand LoadPaymentsCommand { get; }
    public ICommand ProcessPaymentCommand { get; }
    public ICommand RefreshCommand { get; }

    public PaymentManagementViewModel(
        IPaymentService paymentService,
        IBookingService bookingService)
    {
        _paymentService = paymentService;
        _bookingService = bookingService;

        LoadPaymentsCommand = new RelayCommand(async (param) => await LoadPaymentsAsync());
        ProcessPaymentCommand = new RelayCommand(async (param) => await ProcessPaymentAsync());
        RefreshCommand = new RelayCommand(async (param) => await RefreshAllAsync());
    }

    private async Task LoadPaymentsAsync()
    {
        // Note: IPaymentService n'a pas GetAllPaymentsAsync()
        // On charge les paiements par réservation sélectionnée
        if (SelectedBooking != null)
        {
            IsLoading = true;
            try
            {
                var result = await _paymentService.GetPaymentsByBookingIdAsync(SelectedBooking.Id);

                if (result.IsSuccess && result.Value != null)
                {
                    Payments = new ObservableCollection<PaymentDto>(result.Value);
                }
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

    private async Task ProcessPaymentAsync()
    {
        if (SelectedBooking == null) return;

        // TODO: Implémenter boîte de dialogue de paiement
        // var processDto = new ProcessPaymentDto { ... };
        // var result = await _paymentService.ProcessPaymentAsync(processDto);

        // if (result.IsSuccess)
        // {
        //     await LoadPaymentsAsync();
        // }
    }

    private async Task RefreshAllAsync()
    {
        await LoadPaymentsAsync();
        // TODO: Charger les réservations impayées
    }
}