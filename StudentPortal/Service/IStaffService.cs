using StudentPortal.DTO;

namespace StudentPortal.Service
{
    public interface IStaffService
    {
        Task<List<StaffResponseDto>> GetAllAsync();
        Task<StaffResponseDto> GetByIdAsync(int id);
        Task<StaffResponseDto?> CreateAsync(StaffResponseDto dto);
        Task<StaffResponseDto?> UpdateAsync(int id, StaffResponseDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
