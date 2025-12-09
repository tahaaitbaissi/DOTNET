using CarRental.Application.DTOs;
using System.Threading.Tasks;

namespace CarRental.Desktop.Services
{
    public interface IEmailNotificationService
    {
        // Simple envoi
        Task<bool> SendTestEmailAsync(string toEmail);
        Task<bool> SendBookingEmailAsync(BookingDto booking, string clientEmail);
    }
}