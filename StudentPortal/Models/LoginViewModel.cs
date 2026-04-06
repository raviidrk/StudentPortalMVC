using System.ComponentModel.DataAnnotations;

namespace StudentPortal.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 chars")]
        [RegularExpression(@"^[A-Za-z][A-Za-z0-9_]*$", ErrorMessage = "Starts with letter")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Min 6 chars")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
