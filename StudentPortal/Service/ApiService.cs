using StudentPortal.Models;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace StudentPortal.Service
{
    // Services/ApiService.cs
    public class ApiService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        private HttpClient CreateAuthorizedClient()
        {
            var client = _httpClientFactory.CreateClient("MyApi");
            var token = _httpContextAccessor.HttpContext?.Request.Cookies["JWToken"];

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            return client;
        }

        //  DELETE
        public async Task<bool> DeleteAsync(string url)
        {
            var client = CreateAuthorizedClient();
            var response = await client.DeleteAsync(url);

            //  If 401 → refresh token → retry once
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var refreshed = await TryRefreshTokenAsync();

                if (!refreshed)
                    throw new UnauthorizedAccessException(); // refresh failed → redirect to login

                //  new client with refreshed token
                client = CreateAuthorizedClient();
                response = await client.DeleteAsync(url); // no body → no content issue

                //  still 401 after refresh → session truly expired
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new UnauthorizedAccessException();
            }

            return response.IsSuccessStatusCode;
        }

        //  GET
        public async Task<T?> GetAsync<T>(string url)
        {
            var client = CreateAuthorizedClient();
            var response = await client.GetAsync(url);

            //  If 401 → refresh token → retry once
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var refreshed = await TryRefreshTokenAsync();

                if (!refreshed)
                    throw new UnauthorizedAccessException(); // refresh failed → redirect to login

                //  new client with refreshed token
                client = CreateAuthorizedClient();
                response = await client.GetAsync(url); // no body → no content issue

                //  still 401 after refresh → session truly expired
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new UnauthorizedAccessException();
            }

            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }


        //  POST
        public async Task<T?> PostAsync<T>(string url, object body)
        {
            var client = CreateAuthorizedClient();
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, jsonContent);

            //  If 401 → refresh token → retry once
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var refreshed = await TryRefreshTokenAsync();

                if (!refreshed)
                    throw new UnauthorizedAccessException(); // refresh failed → redirect to login

                //  Retry with new token
                client = CreateAuthorizedClient(); // new client with updated token from cookie
                jsonContent = new StringContent(   // recreate content — cannot reuse consumed content
                    JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

                response = await client.PostAsync(url, jsonContent);

                //  If still 401 after refresh → session truly expired
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new UnauthorizedAccessException();
            }

            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        //  PUT — added
        public async Task<T?> PutAsync<T>(string url, object body)
        {
            var client = CreateAuthorizedClient();
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

            var response = await client.PutAsync(url, jsonContent);

            //  If 401 → refresh token → retry once
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var refreshed = await TryRefreshTokenAsync();

                if (!refreshed)
                    throw new UnauthorizedAccessException(); // refresh failed → redirect to login

                //  new client with refreshed token from cookie
                client = CreateAuthorizedClient();

                //  recreate content — cannot reuse consumed content
                jsonContent = new StringContent(
                    JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");

                response = await client.PutAsync(url, jsonContent);

                //  still 401 after refresh → session truly expired
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                    throw new UnauthorizedAccessException();
            }

            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }


        
        //  Refresh token — call API and update cookies
        private async Task<bool> TryRefreshTokenAsync()
        {
            try
            {
                // Step 1 — get refresh token from cookie
                var refreshToken = _httpContextAccessor.HttpContext?.Request.Cookies["RefreshToken"];

                if (string.IsNullOrEmpty(refreshToken))
                    return false;

                // Step 2 — call API refresh-token endpoint (no auth header needed)
                var client = _httpClientFactory.CreateClient("MyApi");

                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(new { RefreshToken = refreshToken }),
                    Encoding.UTF8,
                    "application/json");

                var response = await client.PostAsync("auth/refresh-token", jsonContent);

                // Step 3 — if refresh failed (token expired or revoked)
                if (!response.IsSuccessStatusCode)
                    return false;

                // Step 4 — read new tokens from response
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<LoginResponsecs>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (result == null || string.IsNullOrEmpty(result.AccessToken))
                    return false;

                // Step 5 — update cookies with new tokens
                var httpContext = _httpContextAccessor.HttpContext!;

                httpContext.Response.Cookies.Append("JWToken", result.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(30)  // match your token expiry
                });

                httpContext.Response.Cookies.Append("RefreshToken", result.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(7)  // match your refresh token expiry
                });

                return true; //  refresh successful
            }
            catch
            {
                return false; //  refresh failed
            }
        }
    }
}
