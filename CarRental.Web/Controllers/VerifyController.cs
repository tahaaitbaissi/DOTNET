using CarRental.Web.Models.Api;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace CarRental.Web.Controllers
{
    /// <summary>
    /// Controller for verifying booking authenticity via QR codes
    /// </summary>
    public class VerifyController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<VerifyController> _logger;

        public VerifyController(IHttpClientFactory httpClientFactory, ILogger<VerifyController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <summary>
        /// Verify booking authenticity - accessible without authentication
        /// </summary>
        [HttpGet]
        [Route("verify/booking/{id}")]
        public async Task<IActionResult> Booking(long id)
        {
            _logger.LogInformation("Verification request for booking {BookingId}", id);

            try
            {
                // Try to get token from logged-in user (optional)
                var token = User.FindFirst("Token")?.Value;

                var client = _httpClientFactory.CreateClient("BackendApi");

                // Add auth header if available (allows seeing more details)
                if (!string.IsNullOrEmpty(token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                var response = await client.GetAsync($"api/bookings/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var booking = JsonSerializer.Deserialize<BookingDto>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (booking != null)
                    {
                        ViewBag.IsAuthenticated = !string.IsNullOrEmpty(token);
                        ViewBag.VerificationTime = DateTime.UtcNow;
                        return View(booking);
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // Booking exists but requires authentication to view details
                    ViewBag.BookingId = id;
                    ViewBag.RequiresAuth = true;
                    return View("BookingPrivate");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    _logger.LogWarning("Booking {BookingId} not found during verification", id);
                    ViewBag.BookingId = id;
                    return View("BookingNotFound");
                }

                // Other error
                _logger.LogError("Failed to verify booking {BookingId}: {StatusCode}", id, response.StatusCode);
                ViewBag.BookingId = id;
                ViewBag.Error = "Unable to verify booking at this time.";
                return View("VerificationError");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while verifying booking {BookingId}", id);
                ViewBag.BookingId = id;
                ViewBag.Error = "An error occurred during verification.";
                return View("VerificationError");
            }
        }
    }
}
