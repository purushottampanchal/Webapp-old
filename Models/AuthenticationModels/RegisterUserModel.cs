using System.ComponentModel.DataAnnotations;

namespace WebApp.Models
{
    public class RegisterUserModel
    {
        [Required( ErrorMessage="Please enter Name")]
        public string Name { get; set; }


        [Required( ErrorMessage="Please enter Email")]
        public string Email { get; set; }

        [Required( ErrorMessage="Please enter Phone Number")]
        public string PhoneNo { get; set; }

        [Required( ErrorMessage="Please enter UserName")]
        public string UserName { get; set; }

        [Required( ErrorMessage="Please enter Password")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage="Confirm password doesn't match")]
        public string ConfrimPassword { get; set; }
        public string UserRole { get; set; }
    }
}
