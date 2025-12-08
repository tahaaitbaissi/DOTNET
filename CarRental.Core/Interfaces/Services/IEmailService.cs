using CarRental.Core.Entities;
using System.Threading.Tasks;

namespace CarRental.Core.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendBookingConfirmationAsync(Booking booking);
        Task SendBookingCancelledAsync(Booking booking);
        Task SendAccountVerificationEmailAsync(string email, string code);
        Task SendPasswordResetEmailAsync(string email, string token);
    }
}