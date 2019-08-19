namespace NitelikliBilisim.Core.ViewModels.Account
{
    public class ExternalLoginViewModel
    {
        public string ReturnUrl { get; set; }
        public string LoginProvider { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
