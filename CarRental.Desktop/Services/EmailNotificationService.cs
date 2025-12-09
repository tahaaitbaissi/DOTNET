using CarRental.Application.DTOs;
using System;
using System.Threading.Tasks;

namespace CarRental.Desktop.Services
{
    public class EmailNotificationService : IEmailNotificationService
    {
        private readonly IDialogService _dialogService;

        public EmailNotificationService(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public async Task<bool> SendTestEmailAsync(string toEmail)
        {
            try
            {
                // CORRECTION : Utilisez ShowMessageAsync
                await _dialogService.ShowMessageAsync("Email",
                    $"Email test envoyé à {toEmail} (simulé)");

                return true;
            }
            catch (Exception ex)
            {
                // CORRECTION : Utilisez ShowMessageAsync
                await _dialogService.ShowMessageAsync("Erreur", ex.Message);
                return false;
            }
        }

        public async Task<bool> SendBookingEmailAsync(BookingDto booking, string clientEmail)
        {
            try
            {
                // CORRECTION : Utilisez VehicleName et TotalAmount
                var message = $"Email envoyé à {clientEmail} pour la réservation #{booking.Id}\n" +
                             $"Véhicule: {booking.VehicleName}\n" +
                             $"Montant: {booking.TotalAmount:C}";

                // CORRECTION : Utilisez ShowMessageAsync
                await _dialogService.ShowMessageAsync("Notification", message);

                return true;
            }
            catch (Exception ex)
            {
                // CORRECTION : Utilisez ShowMessageAsync
                await _dialogService.ShowMessageAsync("Erreur", ex.Message);
                return false;
            }
        }
    }
}