﻿using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NitelikliBilisim.Core.Abstracts;
using NitelikliBilisim.Core.Entities;

namespace NitelikliBilisim.Data
{
    public class NbDataContext : IdentityDbContext<ApplicationUser, ApplicationRole, string, IdentityUserClaim<string>,
        ApplicationUserRole, IdentityUserLogin<string>,
        IdentityRoleClaim<string>, IdentityUserToken<string>>
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
                ((AuditBase)(entity.Entity)).CreatedUser = userId;
                ((AuditBase)(entity.Entity)).CreatedDate = DateTime.Now;
            }

            selectedEntityList = ChangeTracker.Entries()
                .Where(x => x.Entity is AuditBase && x.State == EntityState.Modified);

            foreach (var entity in selectedEntityList)
            {
                ((AuditBase)(entity.Entity)).UpdatedUser = userId;
                ((AuditBase)(entity.Entity)).UpdatedDate = DateTime.Now;
            }

            return base.SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.EnableAutoHistory(100);

            #region ManyToMany

            builder.Entity<Bridge_EducationTag>()
                .HasKey(x => new { x.Id, x.Id2 });
            builder.Entity<Bridge_EducationEducator>()
                .HasKey(x => new { x.Id, x.Id2 });
            builder.Entity<Bridge_GroupStudent>()
                .HasKey(x => new { x.Id, x.Id2 });
            builder.Entity<WishlistItem>()
                .HasKey(x => new { x.Id, x.Id2 });

            builder.Entity<ApplicationUserRole>(userRole =>
            {
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId });

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });
            #endregion
        }

        public DbSet<AutoHistory> DataHistories { get; set; }
        public DbSet<Education> Educations { get; set; }
        public DbSet<EducationCategory> EducationCategories { get; set; }
        public DbSet<EducationTag> EducationTags { get; set; }
        public DbSet<Bridge_EducationTag> Bridge_EducationTags { get; set; }
        public DbSet<EducationComment> EducationComments { get; set; }
        public DbSet<EducationMedia> EducationMedias { get; set; }
        public DbSet<EducationPart> EducationParts { get; set; }
        public DbSet<EducationGain> EducationGains { get; set; }
        public DbSet<EducationPromotionCode> EducationPromotionCodes { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceAddress> InvoiceAddresses { get; set; }
        public DbSet<WishlistItem> Wishlist { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Educator> Educators { get; set; }
        public DbSet<EducatorSocialMedia> EducatorSocialMedias { get; set; }
        public DbSet<StudentEducationInfo> StudentEducationInfos { get; set; }
        public DbSet<Suggestion> Suggestions { get; set; }
        public DbSet<Bridge_EducationEducator> Bridge_EducationEducators { get; set; }
        public DbSet<EducationGroup> EducationGroups { get; set; }
        public DbSet<Bridge_GroupStudent> Bridge_GroupStudents { get; set; }
        public DbSet<WeekDaysOfGroup> WeekDaysOfGroups { get; set; }
        public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<OnlinePaymentInfo> OnlinePaymentInfos { get; set; }
        public DbSet<OnlinePaymentDetailsInfo> OnlinePaymentDetailsInfos { get; set; }
        public DbSet<TempSaleData> TempSaleData { get; set; }
        public DbSet<GroupLessonDay> GroupLessonDays { get; set; }
    }
}