using CarRental.Core.Interfaces.Services;
using QRCoder;
using System.Threading.Tasks;

namespace CarRental.Infrastructure.Services
{
    /// <summary>
    /// QR code generation service using QRCoder
    /// </summary>
    public class QrCodeService : IQrCodeService
    {
        /// <summary>
        /// Generates a QR code image as PNG byte array
        /// </summary>
        /// <param name="data">The data to encode in the QR code</param>
        /// <returns>PNG image bytes</returns>
        public Task<byte[]> GenerateQrCodeAsync(string data)
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            
            // Generate QR code with 10 pixels per module (size multiplier)
            var qrCodeBytes = qrCode.GetGraphic(10);
            
            return Task.FromResult(qrCodeBytes);
        }
    }
}

