using CarRental.Application.DTOs;
using CarRental.Desktop.Services;
using CarRental.Desktop.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CarRental.Desktop.ViewModels
{
    public class BookingManagementViewModel : ViewModelBase
    {
        private readonly IBookingServiceClient _bookingService;
        private readonly IPrintService _printService; // Ajouté
        private readonly IDialogService _dialogService;

        private ObservableCollection<BookingDto> _bookings = new();
        private BookingDto? _selectedBooking;

        public BookingManagementViewModel(
            IBookingServiceClient bookingService,
            IPrintService printService, // Injection
            IDialogService dialogService)
        {
            _bookingService = bookingService;
            _printService = printService;
            _dialogService = dialogService;

            LoadBookingsCommand = new AsyncRelayCommand(LoadBookingsAsync);
            CancelBookingCommand = new AsyncRelayCommand(CancelBookingAsync, CanInteractWithBooking);
            PrintInvoiceCommand = new AsyncRelayCommand(PrintInvoiceAsync, CanInteractWithBooking); // Ajouté
            RefreshCommand = new AsyncRelayCommand(LoadBookingsAsync);

            _ = LoadBookingsAsync();
        }

        public ObservableCollection<BookingDto> Bookings
        {
            get => _bookings;
            set => SetProperty(ref _bookings, value);
        }

        public BookingDto? SelectedBooking
        {
            get => _selectedBooking;
            set => SetProperty(ref _selectedBooking, value);
        }

        public ICommand LoadBookingsCommand { get; }
        public ICommand CancelBookingCommand { get; }
        public ICommand PrintInvoiceCommand { get; } // Ajouté
        public ICommand RefreshCommand { get; }

        private bool CanInteractWithBooking() => SelectedBooking != null;

        private async Task LoadBookingsAsync()
        {
            try
            {
                IsLoading = true;
                ClearError();

                // Appel direct car le service retourne List<BookingDto>
                var bookings = await _bookingService.GetAllBookingsAsync();
                Bookings = new ObservableCollection<BookingDto>(bookings);
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

        private async Task CancelBookingAsync()
        {
            if (SelectedBooking == null) return;

            var confirmed = await _dialogService.ShowConfirmationAsync(
                "Confirmation",
                $"Annuler la réservation #{SelectedBooking.Id} ?");

            if (confirmed)
            {
                try
                {
                    IsLoading = true;
                    await _bookingService.CancelBookingAsync(SelectedBooking.Id);
                    await LoadBookingsAsync();
                    await _dialogService.ShowMessageAsync("Succès", "Réservation annulée");
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowMessageAsync("Erreur", ex.Message);
                }
                finally
                {
                    IsLoading = false;
                }
            }
        }

        private async Task PrintInvoiceAsync()
        {
            if (SelectedBooking == null) return;

            try
            {
                IsLoading = true;
                var success = await _printService.PrintInvoiceAsync(SelectedBooking);
                if (!success)
                {
                    await _dialogService.ShowMessageAsync("Info", "Impression annulée ou échouée");
                }
            }
            catch (Exception ex)
            {
                await _dialogService.ShowMessageAsync("Erreur", ex.Message);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}