using System.ComponentModel.DataAnnotations;

namespace OMSys.Models
{
    public class Login
    {
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }
}
