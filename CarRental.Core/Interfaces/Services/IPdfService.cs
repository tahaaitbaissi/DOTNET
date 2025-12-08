using CarRental.Core.Entities;
using System.Threading.Tasks;

namespace CarRental.Core.Interfaces.Services
{
    public interface IPdfService
    {
        Task<byte[]> GenerateBookingConfirmationPdfAsync(Booking booking);
        Task<byte[]> GenerateInvoicePdfAsync(Payment payment);
    }
}