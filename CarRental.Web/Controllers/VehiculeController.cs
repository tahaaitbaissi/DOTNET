using Microsoft.AspNetCore.Mvc;
using CarRental.Web.Models.Api;
using System.Net.Http;
using System.Net.Http.Json;

namespace CarRental.Web.Controllers
{
    public class VehiculeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        
        public VehiculeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        
        public async Task<IActionResult> Index(string? marque, decimal? prixMax)
        {
            var client = _httpClientFactory.CreateClient("BackendApi");
            
            // Fix 404: Use 'available' endpoint instead of root 'api/vehicles' which doesn't exist for GetAll.
            // Providing a default search window of 7 days.
            var startDate = DateTime.Today.ToString("O");
            var endDate = DateTime.Today.AddDays(7).ToString("O");
            
            var url = $"api/vehicles/available?startDate={startDate}&endDate={endDate}";
            
             if (prixMax.HasValue)
            {
                url += $"&maxPricePerDay={prixMax.Value}";
            }

            try 
            {
                var vehicules = await client.GetFromJsonAsync<List<VehicleDto>>(url) ?? new List<VehicleDto>();

                // Client-side filtering for Make
                if (!string.IsNullOrEmpty(marque))
                {
                    vehicules = vehicules.Where(v => v.Make.Contains(marque, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                
                return View(vehicules);
            }
            catch (Exception)
            {
                // Fallback empty list on connection error or bad request
               return View(new List<VehicleDto>());
            }
        }
        
        public async Task<IActionResult> Details(int id)
        {
            var client = _httpClientFactory.CreateClient("BackendApi");
            try 
            {
                var vehicule = await client.GetFromJsonAsync<VehicleDto>($"api/vehicles/{id}");
                if (vehicule == null) return NotFound();
                return View(vehicule);
            }
            catch (HttpRequestException)
            {
                return NotFound();
            }
        }
    }
}