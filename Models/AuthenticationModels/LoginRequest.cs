using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class LoginRequest
    {

        [Required]
        public string? UserName { get; set; }

        [Required]
        public string? Password { get; set; }
   //     public string UserRole { get; internal set; }
    }
}
