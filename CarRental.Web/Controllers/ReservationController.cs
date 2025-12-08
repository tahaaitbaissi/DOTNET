using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using CarRental.Web.Models.Api;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CarRental.Web.Controllers
{
    [Authorize]
    public class ReservationController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ReservationController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        // ============================================
        // Create Booking Flow
        // ============================================
        [HttpGet]
        public async Task<IActionResult> Create(long vehicleId, DateTime startDate, DateTime endDate)
        {
            var client = _httpClientFactory.CreateClient("BackendApi");
            try 
            {
                var vehicle = await client.GetFromJsonAsync<VehicleDto>($"api/vehicles/{vehicleId}");
                if (vehicle == null) return NotFound();

                var days = (endDate - startDate).Days;
                if (days < 1) days = 1;
                
                var model = new CreateBookingDto
                {
                    VehicleId = vehicleId,
                    StartDate = startDate,
                    EndDate = endDate,
                    // View properties not in DTO
                };
                
                ViewBag.Vehicle = vehicle;
                ViewBag.TotalAmount = days * vehicle.DailyRate;
                ViewBag.Days = days;
                
                return View(model);
            }
            catch { return NotFound(); }
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingDto model)
        {
            var token = User.FindFirst("Token")?.Value;
            var clientIdStr = User.FindFirst("ClientId")?.Value;

            if (string.IsNullOrEmpty(clientIdStr))
            {
                 // Handling case where User is not a Client? 
                 // Backend Auto-creates client on booking if needed? 
                 // Or we assume all registered users are clients.
                 // Let's assume clientId is in claim.
                 ModelState.AddModelError("", "User profile incomplete. Please contact support.");
                 return View(model);
            }
            
            model.ClientId = long.Parse(clientIdStr);

            var client = _httpClientFactory.CreateClient("BackendApi");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsJsonAsync("api/bookings", model);
            
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            
            ViewBag.Error = "Booking failed. Vehicle might be unavailable.";
            return View("Error"); // Simple error view
        }

        // ============================================
        // My Bookings (History)
        // ============================================
        [HttpGet]
        public async Task<IActionResult> Index()
        {
             var token = User.FindFirst("Token")?.Value;
             var clientIdStr = User.FindFirst("ClientId")?.Value;
             
             var client = _httpClientFactory.CreateClient("BackendApi");
             client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

             try 
             {
                 // Assuming Backend has GetClientBookings endpoint
                 // GET /api/bookings/client/{clientId}
                 var bookings = await client.GetFromJsonAsync<List<BookingDto>>($"api/bookings/client/{clientIdStr}");
                 return View(bookings ?? new List<BookingDto>());
             }
             catch 
             {
                 return View(new List<BookingDto>()); 
             }
        }

        [HttpGet]
        public async Task<IActionResult> Details(long id)
        {
             var token = User.FindFirst("Token")?.Value;
             var client = _httpClientFactory.CreateClient("BackendApi");
             client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
             
             try 
             {
                 var booking = await client.GetFromJsonAsync<BookingDto>($"api/bookings/{id}");
                 return View(booking);
             }
             catch { return NotFound(); }
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(long id)
        {
            var token = User.FindFirst("Token")?.Value;
            var client = _httpClientFactory.CreateClient("BackendApi");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            await client.PostAsync($"api/bookings/{id}/cancel", null);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> DownloadPdf(long id)
        {
            var token = User.FindFirst("Token")?.Value;
            var client = _httpClientFactory.CreateClient("BackendApi");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"api/bookings/{id}/pdf");
            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                return File(stream, "application/pdf", $"Booking_{id}.pdf");
            }
            return NotFound();
        }
    }
}