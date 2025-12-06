using Microsoft.AspNetCore.Mvc;
using CarRental.Web.Models.Api;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Net.Http;
using System.Net.Http.Json;

namespace CarRental.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpClientFactory.CreateClient("BackendApi");
            var response = await client.PostAsJsonAsync("api/Auth/login", model);

            if (response.IsSuccessStatusCode)
            {
                var authResult = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                if (authResult != null)
                {
                    // Create User Session (Claims)
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, authResult.Username),
                        new Claim(ClaimTypes.Email, authResult.Email),
                        new Claim("Token", authResult.Token), // Store JWT in cookie claim for API calls
                        new Claim("UserId", authResult.Id.ToString()),
                        new Claim("ClientId", authResult.ClientId?.ToString() ?? "")
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpClientFactory.CreateClient("BackendApi");
            var response = await client.PostAsJsonAsync("api/Auth/register", model);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Login");
            }

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", "Registration failed: " + error);
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
        
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}