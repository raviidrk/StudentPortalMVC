using Microsoft.AspNetCore.Mvc;
using StudentPortal.Models;
using System.Text;
using System.Text.Json;

namespace StudentPortal.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        private readonly HttpClient _httpClient;

        public AuthController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        // 🔹 Show Login Page
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // 🔹 Handle Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var requestData = new
            {
                userName = model.UserName,
                password = model.Password
            };

            var content = new StringContent(
                JsonSerializer.Serialize(requestData),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync(
                "https://localhost:7060/api/auth/login", // 🔥 Your API URL
                content);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Invalid login");
                return View(model);
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<LoginResponsecs>(responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            //  Store token (Session or Cookie)
            Response.Cookies.Append("JWToken", result.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTimeOffset.UtcNow.AddMinutes(30)
            });

            Response.Cookies.Append("JWToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });


            return RedirectToAction("Index", "Home");
        }
    }
}
