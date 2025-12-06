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

            var url = $"api/vehicles/available?startDate={start:O}&endDate={end:O}";
            
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