﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NitelikliBilisim.Data;

namespace NitelikliBilisim.Data.Migrations
{
    [DbContext(typeof(NbDataContext))]
    [Migration("20191112094739_RebuildDatabase")]
    partial class RebuildDatabase
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Microsoft.EntityFrameworkCore.AutoHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Changed");

                    b.Property<DateTime>("Created");

                    b.Property<int>("Kind");

                    b.Property<string>("RowId")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("TableName")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.ToTable("DataHistories");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.Bridge_EducationCategory", b =>
                {
                    b.Property<Guid>("Id");

                    b.Property<Guid>("Id2");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("CreatedUser")
                        .HasMaxLength(128);

                    b.Property<DateTime?>("UpdatedDate");

                    b.Property<string>("UpdatedUser")
                        .HasMaxLength(128);

                    b.HasKey("Id", "Id2");

                    b.HasIndex("Id2");

                    b.ToTable("Bridge_EducationCategories");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.Education", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("CreatedUser")
                        .HasMaxLength(128);

                    b.Property<byte>("Days");

                    b.Property<string>("Description")
                        .HasMaxLength(512);

                    b.Property<byte>("HoursPerDay");

                    b.Property<string>("Name")
                        .HasMaxLength(128);

                    b.Property<decimal?>("NewPrice")
                        .HasColumnType("decimal(8, 2)");

                    b.Property<decimal?>("OldPrice")
                        .HasColumnType("decimal(8, 2)");

                    b.Property<DateTime?>("UpdatedDate");

                    b.Property<string>("UpdatedUser")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.ToTable("Educations");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.EducationCategory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("BaseCategoryId");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("CreatedUser")
                        .HasMaxLength(128);

                    b.Property<string>("Description")
                        .HasMaxLength(512);

                    b.Property<string>("Name")
                        .HasMaxLength(128);

                    b.Property<DateTime?>("UpdatedDate");

                    b.Property<string>("UpdatedUser")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("BaseCategoryId");

                    b.ToTable("EducationSpecialCategories");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.Educator", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("Biography")
                        .HasMaxLength(8192);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("CreatedUser")
                        .HasMaxLength(128);

                    b.Property<string>("PhotoUrl")
                        .HasMaxLength(256);

                    b.Property<string>("Title")
                        .HasMaxLength(128);

                    b.Property<DateTime?>("UpdatedDate");

                    b.Property<string>("UpdatedUser")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.ToTable("Educator");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.Identity.ApplicationRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.Identity.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("AvatarPath")
                        .HasMaxLength(256);

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("Name")
                        .HasMaxLength(128);

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<string>("Surname")
                        .HasMaxLength(128);

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.Identity.ApplicationUserRole", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.WishlistItem", b =>
                {
                    b.Property<string>("Id");

                    b.Property<Guid>("Id2");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("CreatedUser")
                        .HasMaxLength(128);

                    b.Property<DateTime?>("UpdatedDate");

                    b.Property<string>("UpdatedUser")
                        .HasMaxLength(128);

                    b.HasKey("Id", "Id2");

                    b.HasIndex("Id2");

                    b.ToTable("Wishlist");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("NitelikliBilisim.Core.Entities.Identity.ApplicationRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("NitelikliBilisim.Core.Entities.Identity.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("NitelikliBilisim.Core.Entities.Identity.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("NitelikliBilisim.Core.Entities.Identity.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.Bridge_EducationCategory", b =>
                {
                    b.HasOne("NitelikliBilisim.Core.Entities.EducationCategory", "Category")
                        .WithMany()
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("NitelikliBilisim.Core.Entities.Education", "Education")
                        .WithMany()
                        .HasForeignKey("Id2")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.EducationCategory", b =>
                {
                    b.HasOne("NitelikliBilisim.Core.Entities.EducationCategory", "BaseCategory")
                        .WithMany()
                        .HasForeignKey("BaseCategoryId");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.Educator", b =>
                {
                    b.HasOne("NitelikliBilisim.Core.Entities.Identity.ApplicationUser", "User")
                        .WithOne("Educator")
                        .HasForeignKey("NitelikliBilisim.Core.Entities.Educator", "Id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.Identity.ApplicationUserRole", b =>
                {
                    b.HasOne("NitelikliBilisim.Core.Entities.Identity.ApplicationRole", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("NitelikliBilisim.Core.Entities.Identity.ApplicationUser", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.WishlistItem", b =>
                {
                    b.HasOne("NitelikliBilisim.Core.Entities.Identity.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("NitelikliBilisim.Core.Entities.Education", "Education")
                        .WithMany()
                        .HasForeignKey("Id2")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
