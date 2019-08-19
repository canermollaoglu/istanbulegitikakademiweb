﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using NitelikliBilisim.Data;

namespace NitelikliBilisim.Data.Migrations
{
    [DbContext(typeof(NbDataContext))]
    partial class NbDataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
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

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.Egitici", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(450);

                    b.Property<string>("Biyografi");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("CreatedUser")
                        .HasMaxLength(128);

                    b.Property<string>("DetayFoto")
                        .HasMaxLength(250);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(120);

                    b.Property<DateTime?>("UpdatedDate");

                    b.Property<string>("UpdatedUser")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.ToTable("Egiticiler");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.Egitim", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Aciklama")
                        .IsRequired();

                    b.Property<string>("Ad")
                        .IsRequired()
                        .HasMaxLength(150);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("CreatedUser")
                        .HasMaxLength(128);

                    b.Property<string>("DetayFoto")
                        .HasMaxLength(250);

                    b.Property<string>("EgitimFoto")
                        .HasMaxLength(250);

                    b.Property<decimal?>("EskiFiyat")
                        .HasColumnType("decimal(8, 2)");

                    b.Property<decimal>("Fiyat")
                        .HasColumnType("decimal(8, 2)");

                    b.Property<int>("GunSayisi");

                    b.Property<DateTime?>("UpdatedDate");

                    b.Property<string>("UpdatedUser")
                        .HasMaxLength(128);

                    b.Property<string>("VideoUrl")
                        .HasMaxLength(250);

                    b.HasKey("Id");

                    b.ToTable("Egitimler");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.EgitimDetay", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Baslik")
                        .IsRequired()
                        .HasMaxLength(120);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("CreatedUser")
                        .HasMaxLength(128);

                    b.Property<Guid>("EgitimId");

                    b.Property<string>("Konu");

                    b.Property<int>("Sira");

                    b.Property<DateTime?>("UpdatedDate");

                    b.Property<string>("UpdatedUser")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("EgitimId");

                    b.ToTable("EgitimDetaylari");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.EgitimKategori", b =>
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

                    b.ToTable("EgitimKategoriler");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.EgitimKazanim", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("CreatedUser")
                        .HasMaxLength(128);

                    b.Property<Guid>("EgitimId");

                    b.Property<string>("KazanimAdi");

                    b.Property<string>("Simge");

                    b.Property<int>("Sira");

                    b.Property<DateTime?>("UpdatedDate");

                    b.Property<string>("UpdatedUser")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("EgitimId");

                    b.ToTable("EgitimKazanimlar");
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

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FotoUrl")
                        .HasMaxLength(250);

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<string>("Surname")
                        .HasMaxLength(100);

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

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.Kategori", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Aciklama")
                        .HasMaxLength(500);

                    b.Property<string>("Ad")
                        .IsRequired()
                        .HasMaxLength(150);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("CreatedUser")
                        .HasMaxLength(128);

                    b.Property<string>("DetayFoto")
                        .HasMaxLength(250);

                    b.Property<string>("KategoriFoto")
                        .HasMaxLength(250);

                    b.Property<DateTime?>("UpdatedDate");

                    b.Property<string>("UpdatedUser")
                        .HasMaxLength(128);

                    b.Property<string>("VideoUrl")
                        .HasMaxLength(250);

                    b.HasKey("Id");

                    b.ToTable("Kategoriler");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.MusteriYorum", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("CreatedUser")
                        .HasMaxLength(128);

                    b.Property<Guid>("EgitimId");

                    b.Property<string>("KullaniciId")
                        .IsRequired()
                        .HasMaxLength(128);

                    b.Property<DateTime?>("OnaylanmaTarihi");

                    b.Property<string>("OnaylayanId")
                        .HasMaxLength(128);

                    b.Property<int>("Puan");

                    b.Property<DateTime?>("UpdatedDate");

                    b.Property<string>("UpdatedUser")
                        .HasMaxLength(128);

                    b.Property<string>("Yorum")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.HasKey("Id");

                    b.HasIndex("EgitimId");

                    b.ToTable("MusteriYorumlar");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.Satis", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Adres");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("CreatedUser")
                        .HasMaxLength(128);

                    b.Property<bool>("EFaturaMukellefiMi");

                    b.Property<int>("FaturaTipi");

                    b.Property<string>("FirmaAdi");

                    b.Property<string>("Il");

                    b.Property<string>("Ilce");

                    b.Property<string>("KartBilgi")
                        .HasMaxLength(10);

                    b.Property<string>("PostaKodu");

                    b.Property<DateTime?>("UpdatedDate");

                    b.Property<string>("UpdatedUser")
                        .HasMaxLength(128);

                    b.Property<string>("UserIp")
                        .HasMaxLength(16);

                    b.Property<string>("VergiDairesi");

                    b.Property<string>("VergiNumarasi");

                    b.HasKey("Id");

                    b.ToTable("Satislar");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.SatisDetay", b =>
                {
                    b.Property<Guid>("Id");

                    b.Property<Guid>("Id2");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("CreatedUser")
                        .HasMaxLength(128);

                    b.Property<decimal>("Fiyat")
                        .HasColumnType("decimal(8, 2)");

                    b.Property<decimal>("Indirim")
                        .HasColumnType("decimal(3, 3)");

                    b.Property<DateTime?>("UpdatedDate");

                    b.Property<string>("UpdatedUser")
                        .HasMaxLength(128);

                    b.HasKey("Id", "Id2");

                    b.HasIndex("Id2");

                    b.ToTable("SatisDetaylar");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.Sepet", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("CreatedUser")
                        .HasMaxLength(128);

                    b.Property<Guid>("EgitimId");

                    b.Property<Guid?>("SatisId");

                    b.Property<DateTime?>("UpdatedDate");

                    b.Property<string>("UpdatedUser")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("EgitimId");

                    b.HasIndex("SatisId");

                    b.ToTable("Sepetler");
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

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("NitelikliBilisim.Core.Entities.Identity.ApplicationRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

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

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.Egitici", b =>
                {
                    b.HasOne("NitelikliBilisim.Core.Entities.Identity.ApplicationUser", "Kullanici")
                        .WithOne("Egitici")
                        .HasForeignKey("NitelikliBilisim.Core.Entities.Egitici", "Id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.EgitimDetay", b =>
                {
                    b.HasOne("NitelikliBilisim.Core.Entities.Egitim", "Egitim")
                        .WithMany("EgitimDetaylar")
                        .HasForeignKey("EgitimId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.EgitimKategori", b =>
                {
                    b.HasOne("NitelikliBilisim.Core.Entities.Egitim", "Egitim")
                        .WithMany("EgitimKategoriler")
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("NitelikliBilisim.Core.Entities.Kategori", "Kategori")
                        .WithMany("EgitimKategoriler")
                        .HasForeignKey("Id2")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.EgitimKazanim", b =>
                {
                    b.HasOne("NitelikliBilisim.Core.Entities.Egitim", "Egitim")
                        .WithMany("EgitimKazanimlar")
                        .HasForeignKey("EgitimId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.MusteriYorum", b =>
                {
                    b.HasOne("NitelikliBilisim.Core.Entities.Egitim", "Egitim")
                        .WithMany("MusteriYorumlar")
                        .HasForeignKey("EgitimId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.SatisDetay", b =>
                {
                    b.HasOne("NitelikliBilisim.Core.Entities.Satis", "Satis")
                        .WithMany()
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("NitelikliBilisim.Core.Entities.Egitim", "Egitim")
                        .WithMany()
                        .HasForeignKey("Id2")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.Sepet", b =>
                {
                    b.HasOne("NitelikliBilisim.Core.Entities.Egitim", "Egitim")
                        .WithMany()
                        .HasForeignKey("EgitimId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("NitelikliBilisim.Core.Entities.Satis", "Satis")
                        .WithMany()
                        .HasForeignKey("SatisId");
                });
#pragma warning restore 612, 618
        }
    }
}
