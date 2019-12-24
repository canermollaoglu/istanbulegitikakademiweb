using System.ComponentModel.DataAnnotations;

namespace NitelikliBilisim.Core.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        //[Required, EmailAddress]
        //public string Email { get; set; }
        public bool RememberMe { get; set; } = true;
        public string ReturnUrl { get; set; }
    }
}