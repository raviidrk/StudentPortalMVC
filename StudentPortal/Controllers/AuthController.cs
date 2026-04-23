using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using StudentPortal.Models;
using StudentPortal.Service;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace StudentPortal.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ApiService _apiService;
        public AuthController(IHttpClientFactory httpClientFactory , ApiService apiService)
        {
            _httpClientFactory = httpClientFactory;
            _apiService = apiService;
        }
        public IActionResult Index()
        {
            return View();
        }


      
        //  Show Login Page
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        //  Handle Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            //  create client from factory using named client
            var client = _httpClientFactory.CreateClient("MyApi");

            var requestData = new
            {
                userName = model.UserName,
                password = model.Password
            };

            var response = await client.PostAsJsonAsync("auth/login", requestData);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View(model);
            }

            var result = await response.Content
                .ReadFromJsonAsync<LoginResponsecs>(new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (result == null)
            {
                ModelState.AddModelError("", "Something went wrong, please try again");
                return View(model);
            }

            // ✅ Decode JWT and extract claims
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(result.AccessToken);

            var role = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var name = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            var userId = jwt.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

                            // ✅ Build claims and sign in
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, name   ?? ""),
                    new Claim(ClaimTypes.Role, role   ?? ""),
                    new Claim("UserId",        userId ?? ""),
                };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
                });
            //  Store token ( Cookie)
            Response.Cookies.Append("JWToken", result.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTimeOffset.UtcNow.AddMinutes(30)
            });

            Response.Cookies.Append("RefreshToken", result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });


            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var refreshToken = Request.Cookies["RefreshToken"];

                if (!string.IsNullOrEmpty(refreshToken))
                {                  
                    await _apiService.PostAsync<object>("auth/logout", new
                    {
                        RefreshToken = refreshToken
                    });
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Logout error: {ex.Message}");
            }
            finally
            {
                //  always clear cookies
                Response.Cookies.Delete("JWToken");
                Response.Cookies.Delete("RefreshToken");
                HttpContext.Session.Clear();
            }

            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

