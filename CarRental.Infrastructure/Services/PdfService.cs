using CarRental.Core.Entities;
using CarRental.Core.Interfaces.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.IO;
using System.Threading.Tasks;

// Alias to avoid conflict with CarRental.Core.Entities.Document
using PdfDocument = QuestPDF.Fluent.Document;

namespace CarRental.Infrastructure.Services
{
    /// <summary>
    /// PDF generation service using QuestPDF
    /// </summary>
    public class PdfService : IPdfService
    {
        private readonly IQrCodeService _qrCodeService;

        static PdfService()
        {
            // Configure QuestPDF license (Community license for open source)
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public PdfService(IQrCodeService qrCodeService)
        {
            _qrCodeService = qrCodeService;
        }

        public async Task<byte[]> GenerateBookingConfirmationPdfAsync(Booking booking)
        {
            var clientName = booking.Client?.User?.FullName ?? "Valued Customer";
            var vehicleName = booking.Vehicle != null 
                ? $"{booking.Vehicle.Year} {booking.Vehicle.Make} {booking.Vehicle.Model}" 
                : "Vehicle";

            // Generate QR Code
            var verifyUrl = $"https://carrental-web.com/verify/booking/{booking.Id}";
            var qrCodeBytes = await _qrCodeService.GenerateQrCodeAsync(verifyUrl);

            var document = PdfDocument.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    // Header
                    page.Header().Element(c => ComposeHeaderWithQr(c, qrCodeBytes));

                    // Content
                    page.Content().Element(content => ComposeBookingContent(content, booking, clientName, vehicleName));

                    // Footer
                    page.Footer().Element(ComposeFooter);
                });
            });

            using var stream = new MemoryStream();
            document.GeneratePdf(stream);
            return stream.ToArray();
        }

        public Task<byte[]> GenerateInvoicePdfAsync(Payment payment)
        {
            var booking = payment.Booking;
            var clientName = booking?.Client?.User?.FullName ?? "Customer";
            var vehicleName = booking?.Vehicle != null 
                ? $"{booking.Vehicle.Year} {booking.Vehicle.Make} {booking.Vehicle.Model}" 
                : "Vehicle";

            var document = PdfDocument.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(40);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    // Header
                    page.Header().Element(ComposeHeader);

                    // Content
                    page.Content().Element(content => ComposeInvoiceContent(content, payment, booking, clientName, vehicleName));

                    // Footer
                    page.Footer().Element(ComposeFooter);
                });
            });

            using var stream = new MemoryStream();
            document.GeneratePdf(stream);
            return Task.FromResult(stream.ToArray());
        }

        private static void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item()
                        .Text("🚗 CAR RENTAL")
                        .Bold()
                        .FontSize(24)
                        .FontColor(Colors.Blue.Darken2);

                    column.Item()
                        .Text("Your Journey, Our Wheels")
                        .FontSize(10)
                        .FontColor(Colors.Grey.Medium);
                });

                row.ConstantItem(120).Column(column =>
                {
                    column.Item().AlignRight().Text("Car Rental Inc.").Bold();
                    column.Item().AlignRight().Text("123 Main Street");
                    column.Item().AlignRight().Text("City, State 12345");
                    column.Item().AlignRight().Text("contact@carrental.com");
                });
            });

            container.PaddingBottom(20);
        }

        private static void ComposeHeaderWithQr(IContainer container, byte[] qrBytes)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(column =>
                {
                    column.Item()
                        .Text("🚗 CAR RENTAL")
                        .Bold()
                        .FontSize(24)
                        .FontColor(Colors.Blue.Darken2);

                    column.Item()
                        .Text("Your Journey, Our Wheels")
                        .FontSize(10)
                        .FontColor(Colors.Grey.Medium);
                });

                // QR Code
                if (qrBytes != null && qrBytes.Length > 0)
                {
                    row.ConstantItem(80).AlignRight().PaddingRight(10).Image(qrBytes);
                }

                row.ConstantItem(150).Column(column =>
                {
                    column.Item().AlignRight().Text("Car Rental Inc.").Bold();
                    column.Item().AlignRight().Text("123 Main Street");
                    column.Item().AlignRight().Text("City, State 12345");
                    column.Item().AlignRight().Text("contact@carrental.com");
                    column.Item().AlignRight().Text("Scan to Verify").FontSize(8).Italic();
                });
            });

            container.PaddingBottom(20);
        }

        private static void ComposeBookingContent(IContainer container, Booking booking, string clientName, string vehicleName)
        {
            container.Column(column =>
            {
                // Title
                column.Item()
                    .PaddingVertical(10)
                    .BorderBottom(2)
                    .BorderColor(Colors.Blue.Darken2)
                    .Text("BOOKING CONFIRMATION")
                    .Bold()
                    .FontSize(18)
                    .FontColor(Colors.Blue.Darken2);

                column.Item().Height(20);

                // Booking Info Box
                column.Item()
                    .Background(Colors.Grey.Lighten4)
                    .Padding(15)
                    .Column(inner =>
                    {
                        inner.Item().Row(row =>
                        {
                            row.RelativeItem().Text($"Booking ID: #{booking.Id}").Bold();
                            row.RelativeItem().AlignRight().Text($"Date: {DateTime.UtcNow:MMM dd, yyyy}");
                        });
                    });

                column.Item().Height(20);

                // Customer Details
                column.Item().Text("CUSTOMER DETAILS").Bold().FontSize(12);
                column.Item().Height(5);
                column.Item().Text($"Name: {clientName}");
                column.Item().Text($"Email: {booking.Client?.User?.Email ?? "N/A"}");

                column.Item().Height(20);

                // Vehicle Details
                column.Item().Text("VEHICLE DETAILS").Bold().FontSize(12);
                column.Item().Height(5);
                column.Item().Text($"Vehicle: {vehicleName}");
                column.Item().Text($"License Plate: {booking.Vehicle?.LicensePlate ?? "N/A"}");

                column.Item().Height(20);

                // Reservation Details Table
                column.Item().Text("RESERVATION DETAILS").Bold().FontSize(12);
                column.Item().Height(10);

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(3);
                    });

                    // Table rows
                    AddTableRow(table, "Pick-up Date:", booking.StartDate.ToString("dddd, MMMM dd, yyyy"));
                    AddTableRow(table, "Return Date:", booking.EndDate.ToString("dddd, MMMM dd, yyyy"));
                    AddTableRow(table, "Duration:", $"{(booking.EndDate - booking.StartDate).Days} days");
                    AddTableRow(table, "Pick-up Location:", booking.PickUpLocation ?? "N/A");
                    AddTableRow(table, "Drop-off Location:", booking.DropOffLocation ?? "N/A");
                });

                column.Item().Height(20);

                // Pricing
                column.Item()
                    .Background(Colors.Blue.Lighten5)
                    .Padding(15)
                    .Row(row =>
                    {
                        row.RelativeItem().Text("TOTAL AMOUNT").Bold();
                        row.ConstantItem(100)
                            .AlignRight()
                            .Text($"${booking.TotalAmount:N2}")
                            .Bold()
                            .FontSize(16)
                            .FontColor(Colors.Blue.Darken2);
                    });

                column.Item().Height(20);

                // Notes
                if (!string.IsNullOrEmpty(booking.Notes))
                {
                    column.Item().Text("NOTES").Bold().FontSize(12);
                    column.Item().Height(5);
                    column.Item()
                        .Background(Colors.Yellow.Lighten4)
                        .Padding(10)
                        .Text(booking.Notes);
                }

                column.Item().Height(30);

                // Terms
                column.Item()
                    .Text("Please bring a valid driver's license and credit card at pick-up.")
                    .FontSize(9)
                    .FontColor(Colors.Grey.Darken1);
            });
        }

        private static void ComposeInvoiceContent(IContainer container, Payment payment, Booking? booking, string clientName, string vehicleName)
        {
            container.Column(column =>
            {
                // Title
                column.Item()
                    .PaddingVertical(10)
                    .BorderBottom(2)
                    .BorderColor(Colors.Green.Darken2)
                    .Text("INVOICE")
                    .Bold()
                    .FontSize(18)
                    .FontColor(Colors.Green.Darken2);

                column.Item().Height(20);

                // Invoice Info Box
                column.Item()
                    .Background(Colors.Grey.Lighten4)
                    .Padding(15)
                    .Column(inner =>
                    {
                        inner.Item().Row(row =>
                        {
                            row.RelativeItem().Text($"Invoice #: INV-{payment.Id:D6}").Bold();
                            row.RelativeItem().AlignRight().Text($"Date: {payment.CreatedAt:MMM dd, yyyy}");
                        });
                        inner.Item().Row(row =>
                        {
                            row.RelativeItem().Text($"Booking ID: #{booking?.Id ?? 0}");
                            row.RelativeItem().AlignRight().Text($"Status: {payment.Status}");
                        });
                    });

                column.Item().Height(20);

                // Bill To
                column.Item().Text("BILL TO").Bold().FontSize(12);
                column.Item().Height(5);
                column.Item().Text(clientName);
                column.Item().Text(booking?.Client?.User?.Email ?? "N/A");

                column.Item().Height(20);

                // Service Details Table
                column.Item().Text("SERVICE DETAILS").Bold().FontSize(12);
                column.Item().Height(10);

                column.Item().Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(4);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(1);
                        columns.RelativeColumn(1);
                    });

                    // Header
                    table.Header(header =>
                    {
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("Description").Bold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignCenter().Text("Days").Bold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignRight().Text("Rate").Bold();
                        header.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignRight().Text("Amount").Bold();
                    });

                    // Service row
                    var days = booking != null ? (booking.EndDate - booking.StartDate).Days : 1;
                    var dailyRate = days > 0 ? payment.Amount / days : payment.Amount;

                    table.Cell().Padding(5).Text($"Vehicle Rental - {vehicleName}");
                    table.Cell().Padding(5).AlignCenter().Text(days.ToString());
                    table.Cell().Padding(5).AlignRight().Text($"${dailyRate:N2}");
                    table.Cell().Padding(5).AlignRight().Text($"${payment.Amount:N2}");
                });

                column.Item().Height(20);

                // Total
                column.Item()
                    .Background(Colors.Green.Lighten5)
                    .Padding(15)
                    .Row(row =>
                    {
                        row.RelativeItem().Text("TOTAL PAID").Bold();
                        row.ConstantItem(100)
                            .AlignRight()
                            .Text($"${payment.Amount:N2}")
                            .Bold()
                            .FontSize(16)
                            .FontColor(Colors.Green.Darken2);
                    });

                column.Item().Height(10);

                // Payment Details
                column.Item().Row(row =>
                {
                    row.RelativeItem().Text($"Payment Method: {payment.PaymentMethod ?? "N/A"}");
                    row.RelativeItem().AlignRight().Text($"Transaction: {payment.TransactionRef ?? "N/A"}");
                });

                column.Item().Height(30);

                // Thank you
                column.Item()
                    .AlignCenter()
                    .Text("Thank you for your business!")
                    .FontSize(12)
                    .FontColor(Colors.Grey.Darken1);
            });
        }

        private static void AddTableRow(TableDescriptor table, string label, string value)
        {
            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(label);
            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(value);
        }

        private static void ComposeFooter(IContainer container)
        {
            container.AlignCenter().Text(text =>
            {
                text.Span("Page ");
                text.CurrentPageNumber();
                text.Span(" of ");
                text.TotalPages();
                text.Span($" | Generated on {DateTime.UtcNow:MMM dd, yyyy HH:mm} UTC");
            });
        }
    }
}
