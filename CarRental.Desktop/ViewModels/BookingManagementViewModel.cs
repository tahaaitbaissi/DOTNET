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
        private readonly IPrintService _printService;
        private readonly IDialogService _dialogService;

        private ObservableCollection<BookingDto> _bookings = new();
        private BookingDto? _selectedBooking;

        public BookingManagementViewModel(
            IBookingServiceClient bookingService,
            IPrintService printService,
            IDialogService dialogService)
        {
            _bookingService = bookingService;
            _printService = printService;
            _dialogService = dialogService;

            LoadBookingsCommand = new AsyncRelayCommand(LoadBookingsAsync);
            ConfirmBookingCommand = new AsyncRelayCommand(ConfirmBookingAsync, CanConfirmBooking);
            CancelBookingCommand = new AsyncRelayCommand(CancelBookingAsync, CanCancelBooking);
            CompleteBookingCommand = new AsyncRelayCommand(CompleteBookingAsync, CanCompleteBooking);
            PrintInvoiceCommand = new AsyncRelayCommand(PrintInvoiceAsync, CanInteractWithBooking);
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
            set
            {
                if (SetProperty(ref _selectedBooking, value))
                {
                    // Notify commands to re-evaluate their CanExecute
                    (ConfirmBookingCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
                    (CancelBookingCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
                    (CompleteBookingCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
                    (PrintInvoiceCommand as AsyncRelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand LoadBookingsCommand { get; }
        public ICommand ConfirmBookingCommand { get; }
        public ICommand CancelBookingCommand { get; }
        public ICommand CompleteBookingCommand { get; }
        public ICommand PrintInvoiceCommand { get; }
        public ICommand RefreshCommand { get; }

        private bool CanInteractWithBooking() => SelectedBooking != null;
        
        private bool CanConfirmBooking() => SelectedBooking != null && SelectedBooking.Status == "Pending";
        
        private bool CanCancelBooking() => SelectedBooking != null && 
            (SelectedBooking.Status == "Pending" || SelectedBooking.Status == "Confirmed");
        
        private bool CanCompleteBooking() => SelectedBooking != null && SelectedBooking.Status == "Confirmed";

        private async Task LoadBookingsAsync()
        {
            try
            {
                IsLoading = true;
                ClearError();

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

        private async Task ConfirmBookingAsync()
        {
            if (SelectedBooking == null) return;

            var confirmed = await _dialogService.ShowConfirmationAsync(
                "Confirmation",
                $"Confirmer la réservation #{SelectedBooking.Id} ?");

            if (confirmed)
            {
                try
                {
                    IsLoading = true;
                    await _bookingService.ConfirmBookingAsync(SelectedBooking.Id);
                    await LoadBookingsAsync();
                    await _dialogService.ShowMessageAsync("Succès", "Réservation confirmée");
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

        private async Task CompleteBookingAsync()
        {
            if (SelectedBooking == null) return;

            var confirmed = await _dialogService.ShowConfirmationAsync(
                "Compléter la réservation",
                $"Marquer la réservation #{SelectedBooking.Id} comme terminée ?");

            if (confirmed)
            {
                try
                {
                    IsLoading = true;
                    var returnDto = new ReturnVehicleDto(
                        SelectedBooking.Id,
                        DateTime.Now,
                        null,
                        "Retour effectué via Desktop",
                        false,
                        null
                    );
                    await _bookingService.CompleteBookingAsync(SelectedBooking.Id, returnDto);
                    await LoadBookingsAsync();
                    await _dialogService.ShowMessageAsync("Succès", "Réservation terminée");
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