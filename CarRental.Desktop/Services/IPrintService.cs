using CarRental.Application.DTOs;
using System.Threading.Tasks;

namespace CarRental.Desktop.Services
{
    public interface IPrintService
    {
        // PDF simple
        Task<bool> PrintBookingAsync(BookingDto booking);
        Task<bool> PrintInvoiceAsync(BookingDto booking);

        // QR Code simple
        Task<string> GenerateQrCodeForBookingAsync(BookingDto booking);

        // Impression directe
        Task<bool> QuickPrintAsync(string content, string title = "Document");
    }
}