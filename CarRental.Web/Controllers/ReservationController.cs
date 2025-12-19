using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using CarRental.Web.Models.Api;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text.Json;

namespace CarRental.Web.Controllers
{
    [Authorize]
    public class ReservationController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ReservationController> _logger;

        public ReservationController(IHttpClientFactory httpClientFactory, ILogger<ReservationController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        // ============================================
        // Create Booking Flow
        // ============================================
        [HttpGet]
        public async Task<IActionResult> Create(long vehicleId, DateTime startDate, DateTime endDate)
        {
            var client = _httpClientFactory.CreateClient("BackendApi");

            _logger.LogInformation("Creating booking for vehicle {VehicleId} from {StartDate} to {EndDate}",
                vehicleId, startDate, endDate);

            try
            {
                var response = await client.GetAsync($"api/vehicles/{vehicleId}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Vehicle {VehicleId} not found or not available", vehicleId);
                    return NotFound();
                }

                var vehicle = await response.Content.ReadFromJsonAsync<VehicleDto>();

                if (vehicle == null)
                {
                    _logger.LogWarning("Vehicle DTO is null for ID: {VehicleId}", vehicleId);
                    return NotFound();
                }

                var days = (endDate - startDate).Days;
                if (days < 1) days = 1;

                var model = new CreateBookingDto
                {
                    VehicleId = vehicleId,
                    StartDate = startDate,
                    EndDate = endDate,
                    PickUpLocation = string.Empty,
                    DropOffLocation = string.Empty
                };

                ViewBag.Vehicle = vehicle;
                ViewBag.TotalAmount = days * vehicle.DailyRate;
                ViewBag.Days = days;

                _logger.LogInformation("Booking form loaded successfully for vehicle {Make} {Model}",
                    vehicle.Make, vehicle.Model);

                return View(model);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error when loading booking form for vehicle {VehicleId}", vehicleId);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when loading booking form for vehicle {VehicleId}", vehicleId);
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBookingDto model)
        {
            _logger.LogInformation("Processing booking submission for vehicle {VehicleId}", model.VehicleId);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model validation failed for booking");

                // Reload vehicle data for the view
                try
                {
                    var vehicleClient = _httpClientFactory.CreateClient("BackendApi");
                    var vehicleResponse = await vehicleClient.GetAsync($"api/vehicles/{model.VehicleId}");
                    if (vehicleResponse.IsSuccessStatusCode)
                    {
                        var vehicle = await vehicleResponse.Content.ReadFromJsonAsync<VehicleDto>();
                        if (vehicle != null)
                        {
                            var days = (model.EndDate - model.StartDate).Days;
                            if (days < 1) days = 1;
                            ViewBag.Vehicle = vehicle;
                            ViewBag.TotalAmount = days * vehicle.DailyRate;
                            ViewBag.Days = days;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error reloading vehicle data for validation error display");
                }

                return View(model);
            }

            var token = User.FindFirst("Token")?.Value;
            var clientIdStr = User.FindFirst("ClientId")?.Value;

            if (string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("User token not found in claims");
                ModelState.AddModelError("", "Session expirée. Veuillez vous reconnecter.");
                return View(model);
            }

            if (string.IsNullOrEmpty(clientIdStr))
            {
                _logger.LogWarning("ClientId not found in user claims for user {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                ModelState.AddModelError("", "Profil utilisateur incomplet. Veuillez contacter le support.");
                return View(model);
            }

            model.ClientId = long.Parse(clientIdStr);

            var client = _httpClientFactory.CreateClient("BackendApi");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            _logger.LogInformation("Submitting booking to API for client {ClientId}, vehicle {VehicleId}",
                model.ClientId, model.VehicleId);
            _logger.LogDebug("Booking details - PickUp: {PickUp}, DropOff: {DropOff}, StartDate: {Start}, EndDate: {End}",
                model.PickUpLocation, model.DropOffLocation, model.StartDate, model.EndDate);

            try
            {
                var response = await client.PostAsJsonAsync("api/bookings", model);

                if (response.IsSuccessStatusCode)
                {
                    var bookingResult = await response.Content.ReadFromJsonAsync<BookingDto>();
                    _logger.LogInformation("Booking created successfully with ID {BookingId}", bookingResult?.Id);

                    TempData["SuccessMessage"] = "Réservation confirmée avec succès !";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Booking creation failed with status {StatusCode}. Error: {Error}",
                        response.StatusCode, errorContent);

                    // Try to parse validation errors
                    try
                    {
                        var errorObj = JsonSerializer.Deserialize<JsonElement>(errorContent);
                        if (errorObj.TryGetProperty("errors", out var errors))
                        {
                            foreach (var error in errors.EnumerateObject())
                            {
                                var errorMessages = error.Value.EnumerateArray()
                                    .Select(e => e.GetString())
                                    .Where(s => s != null);
                                foreach (var msg in errorMessages)
                                {
                                    ModelState.AddModelError(error.Name, msg!);
                                }
                            }
                        }
                        else if (errorObj.TryGetProperty("detail", out var detail))
                        {
                            ModelState.AddModelError("", detail.GetString() ?? "Erreur lors de la création de la réservation");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Erreur lors de la réservation. Le véhicule n'est peut-être plus disponible.");
                        }
                    }
                    catch
                    {
                        ModelState.AddModelError("", $"Erreur lors de la réservation (Code: {response.StatusCode})");
                    }

                    // Reload vehicle data
                    var vehicleResponse = await client.GetAsync($"api/vehicles/{model.VehicleId}");
                    if (vehicleResponse.IsSuccessStatusCode)
                    {
                        var vehicle = await vehicleResponse.Content.ReadFromJsonAsync<VehicleDto>();
                        if (vehicle != null)
                        {
                            var days = (model.EndDate - model.StartDate).Days;
                            if (days < 1) days = 1;
                            ViewBag.Vehicle = vehicle;
                            ViewBag.TotalAmount = days * vehicle.DailyRate;
                            ViewBag.Days = days;
                        }
                    }

                    return View(model);
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error when creating booking");
                ModelState.AddModelError("", "Impossible de se connecter au serveur. Veuillez réessayer.");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when creating booking");
                ModelState.AddModelError("", "Une erreur inattendue s'est produite. Veuillez réessayer.");
                return View(model);
            }
        }

        // ============================================
        // My Bookings (History)
        // ============================================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var token = User.FindFirst("Token")?.Value;
            var clientIdStr = User.FindFirst("ClientId")?.Value;

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(clientIdStr))
            {
                _logger.LogWarning("User authentication incomplete - redirecting to login");
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("BackendApi");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            _logger.LogInformation("Fetching bookings for client {ClientId}", clientIdStr);

            try
            {
                var response = await client.GetAsync($"api/bookings/client/{clientIdStr}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to fetch bookings: {StatusCode}", response.StatusCode);
                    ViewBag.ErrorMessage = "Impossible de charger vos réservations.";
                    return View(new List<BookingDto>());
                }

                var bookings = await response.Content.ReadFromJsonAsync<List<BookingDto>>();

                _logger.LogInformation("Retrieved {Count} bookings for client {ClientId}",
                    bookings?.Count ?? 0, clientIdStr);

                return View(bookings ?? new List<BookingDto>());
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error when fetching bookings for client {ClientId}", clientIdStr);
                ViewBag.ErrorMessage = "Erreur de connexion au serveur.";
                return View(new List<BookingDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when fetching bookings for client {ClientId}", clientIdStr);
                ViewBag.ErrorMessage = "Une erreur inattendue s'est produite.";
                return View(new List<BookingDto>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(long id)
        {
            var token = User.FindFirst("Token")?.Value;

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("BackendApi");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            _logger.LogInformation("Fetching booking details for booking {BookingId}", id);

            try
            {
                var response = await client.GetAsync($"api/bookings/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Booking {BookingId} not found or unauthorized", id);
                    return NotFound();
                }

                var booking = await response.Content.ReadFromJsonAsync<BookingDto>();

                if (booking == null)
                {
                    _logger.LogWarning("Booking DTO is null for ID: {BookingId}", id);
                    return NotFound();
                }

                _logger.LogInformation("Successfully retrieved booking {BookingId}", id);
                return View(booking);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error when fetching booking {BookingId}", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when fetching booking {BookingId}", id);
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(long id)
        {
            var token = User.FindFirst("Token")?.Value;
            var clientIdStr = User.FindFirst("ClientId")?.Value;

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("BackendApi");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            _logger.LogInformation("Attempting to cancel booking {BookingId} for client {ClientId}", id, clientIdStr);

            try
            {
                var response = await client.PutAsync($"api/bookings/{id}/cancel?clientId={clientIdStr}", null);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Booking {BookingId} cancelled successfully", id);
                    TempData["SuccessMessage"] = "Réservation annulée avec succès.";
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Failed to cancel booking {BookingId}: {StatusCode} - {Error}",
                        id, response.StatusCode, errorContent);
                    TempData["ErrorMessage"] = "Impossible d'annuler la réservation. Elle a peut-être déjà commencé.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when cancelling booking {BookingId}", id);
                TempData["ErrorMessage"] = "Une erreur s'est produite lors de l'annulation.";
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> DownloadPdf(long id)
        {
            var token = User.FindFirst("Token")?.Value;

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var client = _httpClientFactory.CreateClient("BackendApi");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            _logger.LogInformation("Downloading PDF for booking {BookingId}", id);

            try
            {
                var response = await client.GetAsync($"api/bookings/{id}/pdf");

                if (response.IsSuccessStatusCode)
                {
                    var pdfBytes = await response.Content.ReadAsByteArrayAsync();
                    _logger.LogInformation("PDF generated successfully for booking {BookingId}", id);
                    return File(pdfBytes, "application/pdf", $"Reservation_{id}.pdf");
                }
                else
                {
                    _logger.LogWarning("Failed to generate PDF for booking {BookingId}: {StatusCode}",
                        id, response.StatusCode);
                    TempData["ErrorMessage"] = "Impossible de générer le PDF.";
                    return RedirectToAction("Details", new { id });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when downloading PDF for booking {BookingId}", id);
                TempData["ErrorMessage"] = "Erreur lors du téléchargement du PDF.";
                return RedirectToAction("Details", new { id });
            }
        }
    }
}
