using Microsoft.AspNetCore.Mvc;
using CarRental.Web.Models.Api;
using System.Net.Http;
using System.Net.Http.Json;

namespace CarRental.Web.Controllers
{
    public class VehiculeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<VehiculeController> _logger;

        public VehiculeController(IHttpClientFactory httpClientFactory, ILogger<VehiculeController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<IActionResult> Index(string? marque, decimal? prixMax, DateTime? startDate, DateTime? endDate)
        {
            var client = _httpClientFactory.CreateClient("BackendApi");

            // Default to today + 7 days if not specified
            var start = startDate ?? DateTime.Today;
            var end = endDate ?? DateTime.Today.AddDays(7);

            // Pass filters back to View for UI state
            ViewBag.StartDate = start.ToString("yyyy-MM-dd");
            ViewBag.EndDate = end.ToString("yyyy-MM-dd");
            ViewBag.Marque = marque;
            ViewBag.PrixMax = prixMax;

            // Build URL with proper date format (ISO 8601)
            var url = $"api/vehicles/available?startDate={start:yyyy-MM-dd}T00:00:00&endDate={end:yyyy-MM-dd}T23:59:59";

            if (prixMax.HasValue)
            {
                url += $"&maxPricePerDay={prixMax.Value}";
            }

            _logger.LogInformation("Fetching vehicles from: {Url}", url);

            try
            {
                var response = await client.GetAsync(url);

                _logger.LogInformation("API Response Status: {StatusCode}", response.StatusCode);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("API Error: {StatusCode} - {Content}", response.StatusCode, errorContent);
                    ViewBag.ErrorMessage = $"Erreur lors de la récupération des véhicules: {response.StatusCode}";
                    return View(new List<VehicleDto>());
                }

                var vehicules = await response.Content.ReadFromJsonAsync<List<VehicleDto>>() ?? new List<VehicleDto>();

                _logger.LogInformation("Received {Count} vehicles from API", vehicules.Count);

                // Client-side filtering for Make
                if (!string.IsNullOrEmpty(marque))
                {
                    vehicules = vehicules.Where(v => v.Make.Contains(marque, StringComparison.OrdinalIgnoreCase)).ToList();
                    _logger.LogInformation("Filtered to {Count} vehicles by make: {Make}", vehicules.Count, marque);
                }

                return View(vehicules);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed when fetching vehicles");
                ViewBag.ErrorMessage = "Impossible de se connecter à l'API. Vérifiez que le serveur est démarré.";
                return View(new List<VehicleDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when fetching vehicles");
                ViewBag.ErrorMessage = "Une erreur inattendue s'est produite.";
                return View(new List<VehicleDto>());
            }
        }

        public async Task<IActionResult> Details(long id)
        {
            var client = _httpClientFactory.CreateClient("BackendApi");

            _logger.LogInformation("Fetching vehicle details for ID: {Id}", id);

            try
            {
                var response = await client.GetAsync($"api/vehicles/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Vehicle not found: {Id} - Status: {StatusCode}", id, response.StatusCode);
                    return NotFound();
                }

                var vehicule = await response.Content.ReadFromJsonAsync<VehicleDto>();

                if (vehicule == null)
                {
                    _logger.LogWarning("Vehicle DTO is null for ID: {Id}", id);
                    return NotFound();
                }

                _logger.LogInformation("Successfully fetched vehicle: {Make} {Model}", vehicule.Make, vehicule.Model);
                return View(vehicule);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed when fetching vehicle {Id}", id);
                ViewBag.ErrorMessage = "Impossible de se connecter à l'API.";
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when fetching vehicle {Id}", id);
                return NotFound();
            }
        }

        // Diagnostic endpoint to test API connectivity
        public async Task<IActionResult> TestApi()
        {
            var client = _httpClientFactory.CreateClient("BackendApi");
            var diagnostics = new Dictionary<string, string>();

            try
            {
                // Test 1: Base URL
                diagnostics["BaseAddress"] = client.BaseAddress?.ToString() ?? "Not configured";

                // Test 2: Simple GET to check if API is running
                var healthResponse = await client.GetAsync("api/vehicles");
                diagnostics["API Health Check"] = $"Status: {healthResponse.StatusCode}";

                // Test 3: Get available vehicles
                var startDate = DateTime.Today.ToString("yyyy-MM-dd") + "T00:00:00";
                var endDate = DateTime.Today.AddDays(7).ToString("yyyy-MM-dd") + "T23:59:59";
                var url = $"api/vehicles/available?startDate={startDate}&endDate={endDate}";
                diagnostics["Request URL"] = url;

                var response = await client.GetAsync(url);
                diagnostics["Available Vehicles Response"] = $"Status: {response.StatusCode}";

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    diagnostics["Response Length"] = $"{content.Length} characters";

                    var vehicles = await response.Content.ReadFromJsonAsync<List<VehicleDto>>();
                    diagnostics["Vehicles Count"] = vehicles?.Count.ToString() ?? "null";

                    if (vehicles?.Any() == true)
                    {
                        var first = vehicles.First();
                        diagnostics["First Vehicle"] = $"{first.Make} {first.Model} - {first.DailyRate:C}";
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    diagnostics["Error Content"] = errorContent.Length > 500 ? errorContent.Substring(0, 500) : errorContent;
                }
            }
            catch (Exception ex)
            {
                diagnostics["Exception"] = ex.Message;
                diagnostics["Exception Type"] = ex.GetType().Name;
                var stackTrace = ex.StackTrace ?? "N/A";
                diagnostics["Stack Trace"] = stackTrace.Length > 500 ? stackTrace.Substring(0, 500) : stackTrace;
            }

            ViewBag.Diagnostics = diagnostics;
            return View();
        }
    }
}
