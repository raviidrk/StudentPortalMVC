using System.ComponentModel.DataAnnotations;

namespace StudentPortal.DTO
{
    public class StaffResponseDto
    {
        public int Id { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Role { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public string? Password { get; set; }
    }
}
