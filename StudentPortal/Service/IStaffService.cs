using StudentPortal.DTO;

namespace StudentPortal.Service
{
    public interface IStaffService
    {
        Task<List<StaffResponseDto>> GetAllAsync();
        Task<StaffResponseDto> GetByIdAsync(int id);
        Task CreateAsync(StaffResponseDto dto);
        Task UpdateAsync(int id, StaffResponseDto dto);
        Task DeleteAsync(int id);
    }
}
