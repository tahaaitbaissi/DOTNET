using Microsoft.AspNetCore.Mvc;
using CarRental.Web.Models.Api;
using System.Net.Http;
using System.Net.Http.Json;

namespace CarRental.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        
        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        
        public async Task<IActionResult> Index()
        {
            var client = _httpClientFactory.CreateClient("BackendApi");
            try 
            {
                var startDate = DateTime.Today.ToString("O");
                var endDate = DateTime.Today.AddDays(7).ToString("O");
                var url = $"api/vehicles/available?startDate={startDate}&endDate={endDate}";

                var vehicules = await client.GetFromJsonAsync<List<VehicleDto>>(url);
                
                return View(vehicules?.Take(3).ToList() ?? new List<VehicleDto>());
            }
            catch
            {
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