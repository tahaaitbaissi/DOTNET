using CarRental.Web.Models.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace CarRental.Web.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IHttpClientFactory httpClientFactory, ILogger<PaymentController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <summary>
        /// Show payment form for a specific booking
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Process(long bookingId)
        {
            var token = User.FindFirst("Token")?.Value;

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            // Get booking details
            var client = _httpClientFactory.CreateClient("BackendApi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await client.GetAsync($"api/bookings/{bookingId}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var booking = JsonSerializer.Deserialize<BookingDto>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (booking == null)
                    {
                        TempData["ErrorMessage"] = "Unable to load booking details.";
                        return RedirectToAction("Index", "Reservation");
                    }

                    // Check if already paid
                    if (booking.IsPaid)
                    {
                        TempData["ErrorMessage"] = "This booking has already been paid.";
                        return RedirectToAction("Details", "Reservation", new { id = bookingId });
                    }

                    // Prepare payment DTO
                    var paymentDto = new ProcessPaymentDto
                    {
                        BookingId = bookingId,
                        Amount = booking.TotalAmount,
                        PaymentMethod = "CreditCard"
                    };

                    ViewBag.Booking = booking;
                    return View(paymentDto);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    TempData["ErrorMessage"] = "Booking not found.";
                    return RedirectToAction("Index", "Reservation");
                }
                else
                {
                    _logger.LogWarning("Failed to load booking {BookingId}: {StatusCode}", bookingId, response.StatusCode);
                    TempData["ErrorMessage"] = "Unable to load booking details.";
                    return RedirectToAction("Index", "Reservation");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading booking {BookingId} for payment", bookingId);
                TempData["ErrorMessage"] = "An error occurred while loading booking details.";
                return RedirectToAction("Index", "Reservation");
            }
        }

        /// <summary>
        /// Process the payment
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Process(ProcessPaymentDto model)
        {
            var token = User.FindFirst("Token")?.Value;

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                // Reload booking details
                await ReloadBookingDetails(model.BookingId, token);
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("BackendApi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            _logger.LogInformation("Processing payment for booking {BookingId}, amount {Amount}, method {Method}",
                model.BookingId, model.Amount, model.PaymentMethod);

            try
            {
                var json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("api/payments/process", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var payment = JsonSerializer.Deserialize<PaymentDto>(responseBody, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    _logger.LogInformation("Payment processed successfully: {PaymentId}, Transaction: {TransactionRef}",
                        payment?.Id, payment?.TransactionRef);

                    TempData["SuccessMessage"] = $"Payment processed successfully! Transaction Reference: {payment?.TransactionRef}";
                    return RedirectToAction("Success", new { id = payment?.Id, bookingId = model.BookingId });
                }
                else
                {
                    string errorMessage = "Payment failed. Please try again.";

                    try
                    {
                        var problemDetails = JsonSerializer.Deserialize<JsonElement>(responseBody);
                        if (problemDetails.TryGetProperty("detail", out var detail))
                        {
                            errorMessage = detail.GetString() ?? errorMessage;
                        }
                        else if (problemDetails.TryGetProperty("title", out var title))
                        {
                            errorMessage = title.GetString() ?? errorMessage;
                        }
                    }
                    catch
                    {
                        // Use default error message
                    }

                    _logger.LogWarning("Payment failed for booking {BookingId}: {StatusCode} - {Error}",
                        model.BookingId, response.StatusCode, errorMessage);

                    ModelState.AddModelError("", errorMessage);
                    await ReloadBookingDetails(model.BookingId, token);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while processing payment for booking {BookingId}", model.BookingId);
                ModelState.AddModelError("", "An unexpected error occurred while processing your payment. Please try again.");
                await ReloadBookingDetails(model.BookingId, token);
                return View(model);
            }
        }

        /// <summary>
        /// Payment success page
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Success(long id, long bookingId)
        {
            var token = User.FindFirst("Token")?.Value;

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("BackendApi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                // Get payment details
                var paymentResponse = await client.GetAsync($"api/payments/{id}");
                PaymentDto? payment = null;

                if (paymentResponse.IsSuccessStatusCode)
                {
                    var json = await paymentResponse.Content.ReadAsStringAsync();
                    payment = JsonSerializer.Deserialize<PaymentDto>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                // Get booking details
                var bookingResponse = await client.GetAsync($"api/bookings/{bookingId}");
                BookingDto? booking = null;

                if (bookingResponse.IsSuccessStatusCode)
                {
                    var json = await bookingResponse.Content.ReadAsStringAsync();
                    booking = JsonSerializer.Deserialize<BookingDto>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                var viewModel = new PaymentResponseViewModel
                {
                    IsSuccess = true,
                    Payment = payment,
                    BookingId = bookingId
                };

                ViewBag.Booking = booking;
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading payment success page for payment {PaymentId}", id);
                TempData["ErrorMessage"] = "Payment was processed, but we couldn't load the details.";
                return RedirectToAction("Details", "Reservation", new { id = bookingId });
            }
        }

        /// <summary>
        /// Get payment history for a booking
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> History(long bookingId)
        {
            var token = User.FindFirst("Token")?.Value;

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("BackendApi");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await client.GetAsync($"api/payments/booking/{bookingId}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var payments = JsonSerializer.Deserialize<List<PaymentDto>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<PaymentDto>();

                    ViewBag.BookingId = bookingId;
                    return View(payments);
                }
                else
                {
                    _logger.LogWarning("Failed to load payment history for booking {BookingId}: {StatusCode}",
                        bookingId, response.StatusCode);
                    TempData["ErrorMessage"] = "Unable to load payment history.";
                    return RedirectToAction("Details", "Reservation", new { id = bookingId });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading payment history for booking {BookingId}", bookingId);
                TempData["ErrorMessage"] = "An error occurred while loading payment history.";
                return RedirectToAction("Details", "Reservation", new { id = bookingId });
            }
        }

        /// <summary>
        /// Helper method to reload booking details into ViewBag
        /// </summary>
        private async Task ReloadBookingDetails(long bookingId, string token)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("BackendApi");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync($"api/bookings/{bookingId}");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var booking = JsonSerializer.Deserialize<BookingDto>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    ViewBag.Booking = booking;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reloading booking details for {BookingId}", bookingId);
            }
        }
    }
}
