using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace NitelikliBilisim.Core.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [StringLength(128)]
        public string Name { get; set; }
        [StringLength(128)]
        public string Surname { get; set; }

        [StringLength(256)]
        public string AvatarPath { get; set; }


        public virtual Educator Educator { get; set; }
        public ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}
