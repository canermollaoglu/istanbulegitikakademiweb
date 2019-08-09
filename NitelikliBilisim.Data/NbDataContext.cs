using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Core.Entities.Identity;

namespace NitelikliBilisim.Data
{
    public class NbDataContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public NbDataContext(DbContextOptions options)
            : base(options)
        {

        }

    }
}
