using Microsoft.AspNetCore.Identity;

namespace NitelikliBilisim.Core.Entities.Identity
{
    public class ApplicationUserRole : IdentityUserRole<string>
    {
        public ApplicationUser User { get; set; }
        public ApplicationRole Role { get; set; }
    }
}
