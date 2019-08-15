using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Entities.Identity;

namespace NitelikliBilisim.Data
{
    public class NbDataContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public NbDataContext(DbContextOptions options)
            : base(options)
        {

        }

        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<Egitim> Egitimler { get; set; }
        public DbSet<EgitimKategori> EgitimKategoriler { get; set; }
        public DbSet<EgitimDetay> EgitimDetaylar { get; set; }
        public DbSet<MusteriYorum> MusteriYorumlar { get; set; }
        public DbSet<EgitimKazanim> EgitimKazanimlar { get; set; }
    }
}
