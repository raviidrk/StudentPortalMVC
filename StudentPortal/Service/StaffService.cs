

using StudentPortal.DTO;

namespace StudentPortal.Service
{
    public class StaffService : IStaffService
    {
        private readonly HttpClient _httpClient;

        public StaffService(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("MyApi");
        }

        public async Task<List<StaffResponseDto>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync("staff");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<StaffResponseDto>>();
        }

        public async Task<StaffResponseDto> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"staff/{id}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<StaffResponseDto>();
        }

        public async Task CreateAsync(StaffResponseDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("staff", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateAsync(int id, StaffResponseDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"staff/{id}", dto);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"staff/{id}");
            response.EnsureSuccessStatusCode();
        }
    }
}
