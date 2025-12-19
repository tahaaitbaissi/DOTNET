using Microsoft.AspNetCore.Mvc;
using CarRental.Web.Models.Api;
using System.Net.Http;
using System.Net.Http.Json;

namespace CarRental.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IHttpClientFactory httpClientFactory, ILogger<HomeController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("BackendApi");

            try
            {
                var startDate = DateTime.Today.ToString("yyyy-MM-dd") + "T00:00:00";
                var endDate = DateTime.Today.AddDays(7).ToString("yyyy-MM-dd") + "T23:59:59";
                var url = $"api/vehicles/available?startDate={startDate}&endDate={endDate}";

                _logger.LogInformation("Fetching vehicles from: {Url}", url);

                var response = await client.GetAsync(url);

                _logger.LogInformation("API Response Status: {StatusCode}", response.StatusCode);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("API Error: {StatusCode} - {Content}", response.StatusCode, errorContent);
                    ViewBag.ErrorMessage = "Impossible de charger les véhicules pour le moment.";
                    return View(new List<VehicleDto>());
                }

                var vehicules = await response.Content.ReadFromJsonAsync<List<VehicleDto>>();

                var vehicleList = vehicules?.Take(6).ToList() ?? new List<VehicleDto>();
                _logger.LogInformation("Displaying {Count} vehicles on home page", vehicleList.Count);

                return View(vehicleList);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request failed when fetching vehicles for home page");
                ViewBag.ErrorMessage = "Le service de location est temporairement indisponible. Veuillez réessayer plus tard.";
                return View(new List<VehicleDto>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error when fetching vehicles for home page");
                ViewBag.ErrorMessage = "Une erreur inattendue s'est produite.";
                return View(new List<VehicleDto>());
            }
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
