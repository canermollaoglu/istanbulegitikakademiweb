using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace NitelikliBilisim.Core.Entities
{
    public class ApplicationRole : IdentityRole
    {
        public ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}
