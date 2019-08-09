using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace NitelikliBilisim.Core.Entities.Identity
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(100)]
        public string Surname { get; set; }
    }
}
