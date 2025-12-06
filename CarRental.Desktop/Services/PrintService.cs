using CarRental.Application.DTOs;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

namespace CarRental.Desktop.Services
{
    public class PrintService : IPrintService
    {
        private readonly IDialogService _dialogService;

        public PrintService(IDialogService dialogService)
        {
            _dialogService = dialogService;
        }

        public async Task<bool> PrintBookingAsync(BookingDto booking)
        {
            try
            {
                var printDialog = new PrintDialog();
                if (printDialog.ShowDialog() != true) return false;

                var document = new FlowDocument
                {
                    PagePadding = new System.Windows.Thickness(40)
                };

                // CORRECTION : Utilisez VehicleName au lieu de VehicleModel
                document.Blocks.Add(new Paragraph(new Run($"BON DE LOCATION #{booking.Id}"))
                {
                    FontSize = 18,
                    FontWeight = System.Windows.FontWeights.Bold,
                    TextAlignment = System.Windows.TextAlignment.Center
                });

                document.Blocks.Add(new Paragraph(new Run($"Client: {booking.ClientName}")));
                document.Blocks.Add(new Paragraph(new Run($"Véhicule: {booking.VehicleName}"))); // VehicleName
                document.Blocks.Add(new Paragraph(new Run($"Du: {booking.StartDate:dd/MM/yyyy}")));
                document.Blocks.Add(new Paragraph(new Run($"Au: {booking.EndDate:dd/MM/yyyy}")));
                // CORRECTION : Utilisez TotalAmount au lieu de TotalPrice
                document.Blocks.Add(new Paragraph(new Run($"Montant: {booking.TotalAmount:C}"))); // TotalAmount
                document.Blocks.Add(new Paragraph(new Run($"Statut: {booking.Status}")));
                document.Blocks.Add(new Paragraph(new Run($"Payé: {(booking.IsPaid ? "Oui" : "Non")}")));

                printDialog.PrintDocument(((IDocumentPaginatorSource)document).DocumentPaginator, "Location");
                return true;
            }
            catch (Exception ex)
            {
                // CORRECTION : Utilisez ShowMessageAsync (doit exister dans IDialogService)
                await _dialogService.ShowMessageAsync("Erreur", $"Impossible d'imprimer: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> PrintInvoiceAsync(BookingDto booking)
        {
            try
            {
                var printDialog = new PrintDialog();
                if (printDialog.ShowDialog() != true) return false;

                var document = new FlowDocument();

                document.Blocks.Add(new Paragraph(new Run($"FACTURE #{booking.Id}")
                {
                    FontSize = 16,
                    FontWeight = System.Windows.FontWeights.Bold
                }));

                document.Blocks.Add(new Paragraph(new Run($" ")));
                document.Blocks.Add(new Paragraph(new Run($"Client: {booking.ClientName}")));
                // CORRECTION : Utilisez VehicleName
                document.Blocks.Add(new Paragraph(new Run($"Véhicule: {booking.VehicleName}")));
                // CORRECTION : Utilisez TotalAmount
                document.Blocks.Add(new Paragraph(new Run($"Montant HT: {booking.TotalAmount:C}")));
                document.Blocks.Add(new Paragraph(new Run($"TVA (20%): {booking.TotalAmount * 0.2m:C}")));
                document.Blocks.Add(new Paragraph(new Run($"Total TTC: {booking.TotalAmount * 1.2m:C}")));

                printDialog.PrintDocument(((IDocumentPaginatorSource)document).DocumentPaginator, "Facture");
                return true;
            }
            catch (Exception ex)
            {
                // CORRECTION : Utilisez ShowMessageAsync
                await _dialogService.ShowMessageAsync("Erreur", ex.Message);
                return false;
            }
        }

        public async Task<string> GenerateQrCodeForBookingAsync(BookingDto booking)
        {
            return await Task.Run(() =>
            {
                // Version simple sans dépendance externe
                var qrData = $"CAR-RENTAL|ID:{booking.Id}|CLIENT:{booking.ClientName}|VEHICULE:{booking.VehicleName}|MONTANT:{booking.TotalAmount}";
                return $"QR:{Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(qrData))}";
            });
        }

        public async Task<bool> QuickPrintAsync(string content, string title = "Document")
        {
            try
            {
                var printDialog = new PrintDialog();
                if (printDialog.ShowDialog() != true) return false;

                var document = new FlowDocument();
                document.Blocks.Add(new Paragraph(new Run(content)));

                printDialog.PrintDocument(((IDocumentPaginatorSource)document).DocumentPaginator, title);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}