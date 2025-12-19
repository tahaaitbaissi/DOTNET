using CarRental.Core.Entities;
using CarRental.Core.Interfaces.Services;
using CarRental.Infrastructure.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace CarRental.Infrastructure.Services
{
    /// <summary>
    /// Email service implementation using MailKit
    /// Supports development mode (console logging) and production mode (actual SMTP)
    /// </summary>
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<EmailService> _logger;

        private readonly IPdfService _pdfService;

        public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger, IPdfService pdfService)
        {
            _settings = settings.Value;
            _logger = logger;
            _pdfService = pdfService;
            _logger.LogInformation("EmailService settings loaded: Host={Host}, Port={Port}, User={User}, DevMode={DevMode}, SSL={SSL}",
                _settings.SmtpHost, _settings.SmtpPort, _settings.Username, _settings.UseDevelopmentMode, _settings.UseSsl);
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            await SendEmailAsync(to, subject, body, null, null);
        }

        public async Task SendEmailAsync(string to, string subject, string body, byte[] pdfAttachment, string pdfFileName)
        {
            if (_settings.UseDevelopmentMode)
            {
                LogEmail(to, subject, body);
                if (pdfAttachment != null)
                {
                    _logger.LogInformation("📎 PDF attachment: {FileName} ({Size} bytes)", pdfFileName, pdfAttachment.Length);
                }
                return;
            }

            await SendEmailViaSmtpAsync(to, subject, body, pdfAttachment, pdfFileName);
        }

        public async Task SendBookingConfirmationAsync(Booking booking)
        {
            // Ensure we have the client email - if not loaded, this is a critical error
            if (booking.Client?.User?.Email == null)
            {
                _logger.LogError("Cannot send confirmation email for booking {BookingId}: Client or User email is null", booking.Id);
                throw new InvalidOperationException($"Booking {booking.Id} does not have a valid client email address.");
            }

            var clientEmail = booking.Client.User.Email;
            var clientName = booking.Client.User.FullName ?? "Valued Customer";
            var vehicleName = booking.Vehicle != null
                ? $"{booking.Vehicle.Year} {booking.Vehicle.Make} {booking.Vehicle.Model}"
                : "Your Vehicle";

            var subject = $"Booking Confirmation - #{booking.Id}";
            var body = GenerateBookingConfirmationBody(booking, clientName, vehicleName);

            // Generate PDF attachment
            byte[] pdfBytes = null;
            try
            {
                pdfBytes = await _pdfService.GenerateBookingConfirmationPdfAsync(booking);
                _logger.LogInformation("PDF generated for booking {BookingId}, size: {Size} bytes", booking.Id, pdfBytes.Length);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to generate PDF for booking {BookingId}, sending email without attachment", booking.Id);
            }

            await SendEmailAsync(clientEmail, subject, body, pdfBytes, $"Booking_{booking.Id}.pdf");

            _logger.LogInformation("Booking confirmation email sent to {Email} for booking {BookingId}", clientEmail, booking.Id);
        }

        public async Task SendBookingCancelledAsync(Booking booking)
        {
            var clientEmail = booking.Client?.User?.Email ?? "unknown@email.com";
            var clientName = booking.Client?.User?.FullName ?? "Valued Customer";
            var vehicleName = booking.Vehicle != null
                ? $"{booking.Vehicle.Year} {booking.Vehicle.Make} {booking.Vehicle.Model}"
                : "Your Vehicle";

            var subject = $"Booking Cancelled - #{booking.Id}";
            var body = GenerateBookingCancelledBody(booking, clientName, vehicleName);

            await SendEmailAsync(clientEmail, subject, body);
        }

        public async Task SendAccountVerificationEmailAsync(string email, string code)
        {
            var subject = "Verify Your Account - Car Rental";
            var body = GenerateVerificationEmailBody(code);
            await SendEmailAsync(email, subject, body);
        }

        public async Task SendPasswordResetEmailAsync(string email, string token)
        {
            var subject = "Reset Your Password - Car Rental";
            var body = GeneratePasswordResetEmailBody(token);
            await SendEmailAsync(email, subject, body);
        }

        private void LogEmail(string to, string subject, string body)
        {
            _logger.LogInformation(
                "\n" +
                "╔══════════════════════════════════════════════════════════════════╗\n" +
                "║                    📧 EMAIL (DEVELOPMENT MODE)                    ║\n" +
                "╠══════════════════════════════════════════════════════════════════╣\n" +
                "║ To:      {To,-55} ║\n" +
                "║ Subject: {Subject,-55} ║\n" +
                "╠══════════════════════════════════════════════════════════════════╣\n" +
                "║ Body:                                                            ║\n" +
                "╚══════════════════════════════════════════════════════════════════╝\n" +
                "{Body}\n" +
                "════════════════════════════════════════════════════════════════════\n",
                to, subject.Length > 55 ? subject[..52] + "..." : subject, body);
        }

        private async Task SendEmailViaSmtpAsync(string to, string subject, string body, byte[] pdfAttachment = null, string pdfFileName = null)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
                message.To.Add(MailboxAddress.Parse(to));
                message.Subject = subject;

                var builder = new BodyBuilder
                {
                    HtmlBody = body,
                    TextBody = StripHtml(body)
                };

                // Add PDF attachment if provided
                if (pdfAttachment != null && !string.IsNullOrEmpty(pdfFileName))
                {
                    builder.Attachments.Add(pdfFileName, pdfAttachment, new ContentType("application", "pdf"));
                    _logger.LogInformation("Added PDF attachment {FileName} to email", pdfFileName);
                }

                message.Body = builder.ToMessageBody();

                using var client = new SmtpClient();

                var secureSocketOptions = _settings.UseSsl
                    ? SecureSocketOptions.StartTls
                    : SecureSocketOptions.None;

                await client.ConnectAsync(_settings.SmtpHost, _settings.SmtpPort, secureSocketOptions);

                if (!string.IsNullOrEmpty(_settings.Username) && !string.IsNullOrEmpty(_settings.Password))
                {
                    await client.AuthenticateAsync(_settings.Username, _settings.Password);
                }

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Email sent successfully to {To}", to);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}", to);
                throw;
            }
        }

        private static string GenerateBookingConfirmationBody(Booking booking, string clientName, string vehicleName)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: #2563eb; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background: #f9fafb; }}
        .details {{ background: white; padding: 15px; margin: 15px 0; border-radius: 8px; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🚗 Booking Confirmed!</h1>
        </div>
        <div class='content'>
            <p>Dear {clientName},</p>
            <p>Your booking has been confirmed. Here are your reservation details:</p>

            <div class='details'>
                <p><strong>Booking ID:</strong> #{booking.Id}</p>
                <p><strong>Vehicle:</strong> {vehicleName}</p>
                <p><strong>Pick-up Date:</strong> {booking.StartDate:dddd, MMMM dd, yyyy}</p>
                <p><strong>Return Date:</strong> {booking.EndDate:dddd, MMMM dd, yyyy}</p>
                <p><strong>Pick-up Location:</strong> {booking.PickUpLocation}</p>
                <p><strong>Drop-off Location:</strong> {booking.DropOffLocation}</p>
                <p><strong>Total Amount:</strong> ${booking.TotalAmount:N2}</p>
            </div>

            <p>Please bring a valid driver's license and the credit card used for booking.</p>
            <p>Thank you for choosing Car Rental!</p>
        </div>
        <div class='footer'>
            <p>© {DateTime.UtcNow.Year} Car Rental. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }

        private static string GenerateBookingCancelledBody(Booking booking, string clientName, string vehicleName)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: #dc2626; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background: #f9fafb; }}
        .details {{ background: white; padding: 15px; margin: 15px 0; border-radius: 8px; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>❌ Booking Cancelled</h1>
        </div>
        <div class='content'>
            <p>Dear {clientName},</p>
            <p>Your booking has been cancelled. Here are the details of the cancelled reservation:</p>

            <div class='details'>
                <p><strong>Booking ID:</strong> #{booking.Id}</p>
                <p><strong>Vehicle:</strong> {vehicleName}</p>
                <p><strong>Original Pick-up Date:</strong> {booking.StartDate:dddd, MMMM dd, yyyy}</p>
                <p><strong>Original Return Date:</strong> {booking.EndDate:dddd, MMMM dd, yyyy}</p>
            </div>

            <p>If you did not request this cancellation, please contact us immediately.</p>
            <p>We hope to serve you again in the future!</p>
        </div>
        <div class='footer'>
            <p>© {DateTime.UtcNow.Year} Car Rental. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }

        private static string GenerateVerificationEmailBody(string code)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: #4f46e5; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background: #f9fafb; }}
        .code {{ font-size: 32px; font-weight: bold; text-align: center; letter-spacing: 5px; color: #4f46e5; margin: 20px 0; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Verify Your Email</h1>
        </div>
        <div class='content'>
            <p>Welcome to Car Rental! Please use the verification code below to activate your account:</p>

            <div class='code'>{code}</div>

            <p>This code will expire in 15 minutes.</p>
            <p>If you didn't create an account, you can safely ignore this email.</p>
        </div>
        <div class='footer'>
            <p>© {DateTime.UtcNow.Year} Car Rental. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }

        private static string GeneratePasswordResetEmailBody(string token)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: #ef4444; color: white; padding: 20px; text-align: center; }}
        .content {{ padding: 20px; background: #f9fafb; }}
        .code {{ font-size: 24px; font-weight: bold; text-align: center; background: #fee2e2; padding: 15px; border-radius: 8px; margin: 20px 0; word-break: break-all; }}
        .footer {{ text-align: center; padding: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>Reset Your Password</h1>
        </div>
        <div class='content'>
            <p>We received a request to reset your password. Use the token below to set a new password:</p>

            <div class='code'>{token}</div>

            <p>This token is valid for 1 hour.</p>
            <p>If you didn't request a password reset, please ignore this email.</p>
        </div>
        <div class='footer'>
            <p>© {DateTime.UtcNow.Year} Car Rental. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";
        }

        private static string StripHtml(string html)
        {
            // Simple HTML tag removal for plain text version
            return System.Text.RegularExpressions.Regex.Replace(html, "<[^>]*>", " ")
                .Replace("  ", " ")
                .Trim();
        }
    }
}
