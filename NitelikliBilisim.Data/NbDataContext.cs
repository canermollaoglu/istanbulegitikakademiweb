using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Core.Entities.Identity;

namespace NitelikliBilisim.Data
{
    public class OisDataContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public OisDataContext(DbContextOptions options)
            : base(options)
        {

        }

    }
}
