using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ECommerce.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ECommerceAPI");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Email and password are required.";
                return View();
            }

            var loginData = new
            {
                Email = email,
                Password = password
            };

            var response = await _httpClient.PostAsJsonAsync(
                "api/auth/login",
                loginData
            );

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Email or password is incorrect.";
                return View();
            }

            var json = await response.Content
                .ReadFromJsonAsync<JsonElement>();

            if (!json.TryGetProperty("token", out var tokenProperty))
            {
                ViewBag.Error = "Login failed. Token not found.";
                return View();
            }

            var token = tokenProperty.GetString();

            if (string.IsNullOrWhiteSpace(token))
            {
                ViewBag.Error = "Login failed.";
                return View();
            }

            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken;

            try
            {
                jwtToken = handler.ReadJwtToken(token);
            }
            catch (Exception)
            {
                ViewBag.Error = "Invalid login token.";
                return View();
            }

            var userIdClaim = jwtToken.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.NameIdentifier ||
                c.Type == "nameid" ||
                c.Type == "sub" ||
                c.Type == "userId");

            if (userIdClaim == null ||
                !int.TryParse(userIdClaim.Value, out int userId) ||
                userId <= 0)
            {
                ViewBag.Error = "User ID not found in login token.";
                return View();
            }

            HttpContext.Session.SetString("Token", token);
            HttpContext.Session.SetInt32("UserId", userId);

            return RedirectToAction("Index", "Products");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            string name,
            string email,
            string password)
        {
            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "All fields are required.";
                return View();
            }

            var registerData = new
            {
                Name = name,
                Email = email,
                Password = password
            };

            var response = await _httpClient.PostAsJsonAsync(
                "api/auth/register",
                registerData
            );

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Registration failed.";
                return View();
            }

            return RedirectToAction("Login", "Account");
        }
    }
}