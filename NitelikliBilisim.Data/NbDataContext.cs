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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            #region ManyToMany
            builder.Entity<EgitimKategori>()
                    .HasKey(x => new { x.Id, x.Id2 });
            builder.Entity<SatisDetay>()
                .HasKey(x => new { x.Id, x.Id2 });
            #endregion

            builder.Entity<Egitici>()
                .Property(x => x.Id)
                .HasMaxLength(450);

            builder.Entity<ApplicationUser>()
                .HasOne(x => x.Egitici)
                .WithOne(x => x.Kullanici)
                .HasForeignKey<Egitici>(x => x.Id);
        }

        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<Egitim> Egitimler { get; set; }
        public DbSet<EgitimKategori> EgitimKategoriler { get; set; }
        public DbSet<EgitimDetay> EgitimDetaylar { get; set; }
        public DbSet<MusteriYorum> MusteriYorumlar { get; set; }
        public DbSet<EgitimKazanim> EgitimKazanimlar { get; set; }
        public DbSet<Egitici> Egiticiler { get; set; }
    }
}
