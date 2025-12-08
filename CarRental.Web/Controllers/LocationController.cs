using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using CarRental.Web.Models.Api;
using System.Net.Http;
using System.Net.Http.Json;

namespace CarRental.Web.Controllers
{
    [Authorize] // Use cookie auth default
    public class LocationController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        
        public LocationController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        
        private long GetCurrentClientId()
        {
            // We stored ClientId in claims during login
            var clientIdClaim = User.FindFirst("ClientId")?.Value;
            if (long.TryParse(clientIdClaim, out long clientId))
            {
                return clientId;
            }
            return 0; // Or handle error
        }

        private string GetToken()
        {
            return User.FindFirst("Token")?.Value ?? "";
        }

        private HttpClient CreateClientWithAuth()
        {
            var client = _httpClientFactory.CreateClient("BackendApi");
            var token = GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            return client;
        }
        
        public async Task<IActionResult> Index()
        {
            var clientId = GetCurrentClientId();
            var client = CreateClientWithAuth();
            var response = await client.GetAsync($"api/Bookings/client/{clientId}");
            
            if (response.IsSuccessStatusCode)
            {
                 var bookings = await response.Content.ReadFromJsonAsync<List<BookingDto>>();
                 return View(bookings);
            }
            return View(new List<BookingDto>());
        }
        
        [HttpGet]
        public async Task<IActionResult> Create(int vehiculeId)
        {
            var client = CreateClientWithAuth();
            // Fetch vehicle details to show in view
            var vehicule = await client.GetFromJsonAsync<VehicleDto>($"api/vehicles/{vehiculeId}");
            
            if (vehicule == null)
            {
                TempData["Error"] = "Véhicule introuvable.";
                return RedirectToAction("Index", "Vehicule");
            }
            
            if (vehicule.Status != "Available") // Simplified check
            {
                TempData["Error"] = "Ce véhicule n'est pas disponible.";
                return RedirectToAction("Details", "Vehicule", new { id = vehiculeId });
            }
            
            return View(vehicule);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int vehiculeId, DateTime dateDebut, DateTime dateFin)
        {
            try
            {
                var clientId = GetCurrentClientId();
                
                if (dateDebut < DateTime.Today)
                {
                    TempData["Error"] = "La date de début ne peut pas être dans le passé.";
                    return RedirectToAction("Create", new { vehiculeId });
                }
                
                if (dateFin <= dateDebut)
                {
                    TempData["Error"] = "La date de fin doit être postérieure à la date de début.";
                    return RedirectToAction("Create", new { vehiculeId });
                }

                var bookingDto = new CreateBookingDto
                {
                    VehicleId = vehiculeId,
                    ClientId = clientId,
                    StartDate = dateDebut,
                    EndDate = dateFin
                };

                var client = CreateClientWithAuth();
                var response = await client.PostAsJsonAsync("api/Bookings", bookingDto);

                if (response.IsSuccessStatusCode)
                {
                     var booking = await response.Content.ReadFromJsonAsync<BookingDto>();
                     TempData["Success"] = "Votre demande de location a été enregistrée avec succès!";
                     return RedirectToAction("Details", new { id = booking.Id });
                }
                
                var error = await response.Content.ReadAsStringAsync();
                TempData["Error"] = "Erreur lors de la réservation: " + error;
                return RedirectToAction("Create", new { vehiculeId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erreur: {ex.Message}";
                return RedirectToAction("Create", new { vehiculeId });
            }
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                var clientId = GetCurrentClientId();
                var client = CreateClientWithAuth();
                var response = await client.PutAsync($"api/Bookings/{id}/cancel?clientId={clientId}", null);
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Réservation annulée avec succès.";
                    return RedirectToAction("Index");
                }
                
                TempData["Error"] = "Impossible d'annuler la réservation.";
                return RedirectToAction("Details", new { id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Erreur: {ex.Message}";
                return RedirectToAction("Details", new { id });
            }
        }
        
        public async Task<IActionResult> Details(int id)
        {
             var client = CreateClientWithAuth();
             var response = await client.GetAsync($"api/Bookings/{id}");
             
             if (response.IsSuccessStatusCode)
             {
                 var booking = await response.Content.ReadFromJsonAsync<BookingDto>();
                 return View(booking);
             }
             
             return NotFound();
        }

        // Stub for now (Client-side generation later or API endpoint needed)
        public IActionResult DownloadPdf(int id)
        {
            TempData["Error"] = "Download PDF not implemented yet.";
            return RedirectToAction("Details", new { id });
        }
    }
}