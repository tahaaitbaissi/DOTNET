using Microsoft.AspNetCore.Mvc;

namespace CarRental.Web.Controllers
{
    public class ReservationController : Controller
    {
        // Removed Service dependency as it's not available yet via API
        
        [HttpGet]
        public IActionResult Verify(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                ViewBag.Error = "Code de réservation manquant";
                return View();
            }

            // TODO: Implement API Verification endpoint
            ViewBag.Error = "Vérification non disponible pour le moment (API pending)";
            return View();
        }
    }
}