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
                        new Claim(ClaimTypes.Name, authResult.User.Username),
                        new Claim(ClaimTypes.Email, authResult.User.Email),
                        new Claim("Token", authResult.Token), // Store JWT in cookie claim for API calls
                        new Claim("UserId", authResult.User.Id.ToString()),
                        new Claim("ClientId", authResult.User.ClientId?.ToString() ?? "")
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
        
        // ============================================
        // Profile Management
        // ============================================
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var token = User.FindFirst("Token")?.Value;

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login");

            var client = _httpClientFactory.CreateClient("BackendApi");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try 
            {
                var response = await client.GetAsync("api/profile");
                if (response.IsSuccessStatusCode)
                {
                    var profileData = await response.Content.ReadFromJsonAsync<UserProfileDto>();
                    if (profileData != null)
                    {
                        return View(profileData);
                    }
                }
            }
            catch {}

            // Fallback to claims if API fails
            var model = new UserProfileDto
            {
                Username = User.Identity?.Name ?? "",
                Email = User.FindFirst(ClaimTypes.Email)?.Value ?? "",
                FullName = User.Identity?.Name ?? "",
                IsEmailVerified = true
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var token = User.FindFirst("Token")?.Value;

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login");

            var client = _httpClientFactory.CreateClient("BackendApi");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try 
            {
                var response = await client.GetAsync("api/profile");
                if (response.IsSuccessStatusCode)
                {
                    var profileData = await response.Content.ReadFromJsonAsync<UserProfileDto>();
                    if (profileData != null)
                    {
                        return View(profileData);
                    }
                }
            }
            catch {}

            return RedirectToAction("Profile");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserProfileDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var token = User.FindFirst("Token")?.Value;
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login");

            var client = _httpClientFactory.CreateClient("BackendApi");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Map UserProfileDto to backend UpdateClientDto
            var updateDto = new 
            {
                FullName = model.FullName,
                Phone = model.Phone,
                Address = model.Address,
                DriverLicense = model.DriverLicense,
                LicenseExpiry = DateTime.UtcNow.AddYears(5) // Default if not editable
            };

            var response = await client.PutAsJsonAsync("api/profile", updateDto);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Profile updated successfully!";
                return RedirectToAction("Profile");
            }

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", "Update failed: " + error);
            return View(model);
        }


        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var token = User.FindFirst("Token")?.Value;
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login");

            var client = _httpClientFactory.CreateClient("BackendApi");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.PostAsJsonAsync("api/Auth/change-password", new 
            {
                CurrentPassword = model.CurrentPassword,
                NewPassword = model.NewPassword
            });

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Password changed successfully!";
                return RedirectToAction("Profile");
            }

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", "Password change failed: " + error);
            return View(model);
        }

        // ============================================
        // Email Verification
        // ============================================
        [HttpGet]
        public IActionResult VerifyEmail(string email)
        {
            // Allow user to manually enter code if link fails or redirect to code entry
            return View(new VerifyEmailDto { Email = email });
        }

        [HttpPost]
        public async Task<IActionResult> VerifyEmail(VerifyEmailDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpClientFactory.CreateClient("BackendApi");
            var response = await client.PostAsJsonAsync("api/Auth/verify", model);

            if (response.IsSuccessStatusCode)
            {
                ViewBag.Success = "Email verified successfully! You may now login.";
                return RedirectToAction("Login");
            }

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", "Verification failed: " + error);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResendVerification()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return RedirectToAction("Login");

            var client = _httpClientFactory.CreateClient("BackendApi");
            var response = await client.PostAsJsonAsync("api/Auth/resend-verification", new { Email = email });

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Verification email sent!";
            }
            else
            {
                 TempData["Error"] = "Failed to send verification email.";
            }

            return RedirectToAction("Profile");
        }

        // ============================================
        // Password Recovery
        // ============================================
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpClientFactory.CreateClient("BackendApi");
            var response = await client.PostAsJsonAsync("api/Auth/forgot-password", model);

            if (response.IsSuccessStatusCode)
            {
                // Always success message for security
                ViewBag.Message = "If an account exists, a reset link has been sent.";
                ViewBag.Email = model.Email;
                return View("ForgotPasswordConfirmation");
            }

            ModelState.AddModelError("", "Request failed. Please try again.");
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            return View(new ResetPasswordDto { Email = email, Token = token });
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _httpClientFactory.CreateClient("BackendApi");
            var response = await client.PostAsJsonAsync("api/Auth/reset-password", model);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", "Reset failed: " + error);
            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }        

        
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}