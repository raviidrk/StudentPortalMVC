using System.ComponentModel.DataAnnotations;

namespace StudentPortal.Models
{
    public class StaffViewModel
    {

        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string FullName { get; set; }
        [Required]
        [MaxLength(50)]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }

        [MaxLength(15)]
        public string? PhoneNumber { get; set; }


    }
}
