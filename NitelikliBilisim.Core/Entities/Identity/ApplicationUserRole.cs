using Microsoft.AspNetCore.Identity;

namespace NitelikliBilisim.Core.Entities
{
    public class ApplicationUserRole : IdentityUserRole<string>
    {
        public ApplicationUser User { get; set; }
        public ApplicationRole Role { get; set; }
    }
}
