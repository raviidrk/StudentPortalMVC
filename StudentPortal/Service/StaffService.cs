

using StudentPortal.DTO;

namespace StudentPortal.Service
{
    public class StaffService : IStaffService
    {
        private readonly ApiService _apiService;
        private const string Base = "staff";

        public StaffService(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<List<StaffResponseDto>> GetAllAsync()
        {
            return await _apiService.GetAsync<List<StaffResponseDto>>(Base)
                   ?? new List<StaffResponseDto>();
        }

        public async Task<StaffResponseDto> GetByIdAsync(int id)
        {
            return await _apiService.GetAsync<StaffResponseDto>($"{Base}/{id}");
        }

        public async Task<StaffResponseDto?> CreateAsync(StaffResponseDto dto)
        {
            return await _apiService.PostAsync<StaffResponseDto>(Base, dto);
        }

        public async Task<StaffResponseDto?> UpdateAsync(int id, StaffResponseDto dto)
        {
            return await _apiService.PutAsync<StaffResponseDto>($"{Base}/{id}", dto);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _apiService.DeleteAsync($"{Base}/{id}");
        }
    }
}
