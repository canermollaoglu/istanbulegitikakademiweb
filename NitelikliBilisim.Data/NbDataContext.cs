using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Core.Abstracts;
using NitelikliBilisim.Core.Entities;
using NitelikliBilisim.Core.Entities.Identity;

namespace NitelikliBilisim.Data
{
    public class NbDataContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NbDataContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            this.Database.Migrate();
        }

        public override int SaveChanges()
        {
            //TODO: jwt yapınca kontrol et!!!!
            var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var selectedEntityList = ChangeTracker.Entries()
                .Where(x => x.Entity is AuditBase && x.State == EntityState.Added);

            foreach (var entity in selectedEntityList)
            {
                ((AuditBase) (entity.Entity)).CreatedUser = userId;
                ((AuditBase) (entity.Entity)).CreatedDate = DateTime.Now;
            }

            selectedEntityList = ChangeTracker.Entries()
                .Where(x => x.Entity is AuditBase && x.State == EntityState.Modified);

            foreach (var entity in selectedEntityList)
            {
                ((AuditBase) (entity.Entity)).UpdatedUser = userId;
                ((AuditBase) (entity.Entity)).UpdatedDate = DateTime.Now;
            }
            
            return base.SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.EnableAutoHistory(100);
            #region ManyToMany

            builder.Entity<EgitimKategori>()
                .HasKey(x => new {x.Id, x.Id2});
            builder.Entity<SatisDetay>()
                .HasKey(x => new {x.Id, x.Id2});

            #endregion

            builder.Entity<Egitici>()
                .Property(x => x.Id)
                .HasMaxLength(450);

            builder.Entity<ApplicationUser>()
                .HasOne(x => x.Egitici)
                .WithOne(x => x.Kullanici)
                .HasForeignKey<Egitici>(x => x.Id);
        }

        public DbSet<AutoHistory> DataHistories { get; set; }
        public DbSet<Kategori> Kategoriler { get; set; }
        public DbSet<Egitim> Egitimler { get; set; }
        public DbSet<EgitimKategori> EgitimKategoriler { get; set; }
        public DbSet<EgitimDetay> EgitimDetaylar { get; set; }
        public DbSet<MusteriYorum> MusteriYorumlar { get; set; }
        public DbSet<EgitimKazanim> EgitimKazanimlar { get; set; }
        public DbSet<Egitici> Egiticiler { get; set; }
        public DbSet<Sepet> Sepetler { get; set; }
        public DbSet<Satis> Satislar { get; set; }
        public DbSet<SatisDetay> SatisDetaylar { get; set; }
    }
}