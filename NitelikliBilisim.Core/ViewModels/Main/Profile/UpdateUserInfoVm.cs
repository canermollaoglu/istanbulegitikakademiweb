using Microsoft.AspNetCore.Http;

namespace NitelikliBilisim.Core.ViewModels.Main.Profile
{
    public class UpdateUserInfoVm
    {
        public string UserId { get; set; }
        public IFormFile ProfileImage { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}
