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

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.ApplicationRole", b =>
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

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.ApplicationUser", b =>
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

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.ApplicationUserRole", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.Bridge_EducationTag", b =>
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

                    b.ToTable("Bridge_EducationTags");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.Customer", b =>
                {
                    b.Property<string>("Id");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("CreatedUser")
                        .HasMaxLength(128);

                    b.Property<int>("CustomerType");

                    b.Property<bool>("IsNbuyStudent");

                    b.Property<DateTime?>("UpdatedDate");

                    b.Property<string>("UpdatedUser")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.ToTable("Customers");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.Education", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CategoryId");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("CreatedUser")
                        .HasMaxLength(128);

                    b.Property<byte>("Days");

                    b.Property<string>("Description")
                        .HasMaxLength(512);

                    b.Property<string>("Description2")
                        .HasMaxLength(512);

                    b.Property<byte>("HoursPerDay");

                    b.Property<bool>("IsActive");

                    b.Property<int>("Level");

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

                    b.HasIndex("CategoryId");

                    b.ToTable("Educations");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.EducationCategory", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("BaseCategoryId");

                    b.Property<int>("CategoryType");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("CreatedUser")
                        .HasMaxLength(128);

                    b.Property<string>("Description")
                        .HasMaxLength(512);

                    b.Property<bool>("IsCurrent");

                    b.Property<string>("Name")
                        .HasMaxLength(128);

                    b.Property<DateTime?>("UpdatedDate");

                    b.Property<string>("UpdatedUser")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("BaseCategoryId");

                    b.ToTable("EducationCategories");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.EducationComment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("ApprovalDate")
                        .HasColumnType("datetime2(3)");

                    b.Property<string>("ApproverId")
                        .HasMaxLength(128);

                    b.Property<Guid?>("BaseCommentId");

                    b.Property<string>("CommentatorId")
                        .HasMaxLength(128);

                    b.Property<string>("Content")
                        .HasMaxLength(512);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("CreatedUser")
                        .HasMaxLength(128);

                    b.Property<Guid>("EducationId");

                    b.Property<bool>("IsEducatorComment");

                    b.Property<byte>("Points");

                    b.Property<DateTime?>("UpdatedDate");

                    b.Property<string>("UpdatedUser")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("BaseCommentId");

                    b.HasIndex("EducationId");

                    b.ToTable("EducationComments");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.EducationGain", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("CreatedUser")
                        .HasMaxLength(128);

                    b.Property<Guid>("EducationId");

                    b.Property<string>("Gain")
                        .HasMaxLength(512);

                    b.Property<string>("Title")
                        .HasMaxLength(128);

                    b.Property<DateTime?>("UpdatedDate");

                    b.Property<string>("UpdatedUser")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("EducationId");

                    b.ToTable("EducationGains");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.EducationMedia", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("CreatedUser")
                        .HasMaxLength(128);

                    b.Property<Guid>("EducationId");

                    b.Property<string>("FileUrl")
                        .HasMaxLength(256);

                    b.Property<int>("MediaType");

                    b.Property<DateTime?>("UpdatedDate");

                    b.Property<string>("UpdatedUser")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("EducationId");

                    b.ToTable("EducationMediaItems");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.EducationPart", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("BasePartId");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("CreatedUser")
                        .HasMaxLength(128);

                    b.Property<byte>("Duration");

                    b.Property<Guid>("EducationId");

                    b.Property<byte>("Order");

                    b.Property<string>("Title")
                        .HasMaxLength(128);

                    b.Property<DateTime?>("UpdatedDate");

                    b.Property<string>("UpdatedUser")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("BasePartId");

                    b.HasIndex("EducationId");

                    b.ToTable("EducationParts");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.EducationPromotionCode", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code")
                        .HasMaxLength(7);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("CreatedUser")
                        .HasMaxLength(128);

                    b.Property<bool>("IsUsed");

                    b.Property<byte>("OffPercentage");

                    b.Property<byte>("TimesUsable");

                    b.Property<DateTime?>("UpdatedDate");

                    b.Property<string>("UpdatedUser")
                        .HasMaxLength(128);

                    b.Property<DateTime>("ValidThru");

                    b.HasKey("Id");

                    b.ToTable("EducationPromotionCodes");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.EducationTag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("BaseTagId");

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

                    b.HasIndex("BaseTagId");

                    b.ToTable("EducationTags");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.Educator", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("Biography")
                        .HasMaxLength(8192);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("CreatedUser")
                        .HasMaxLength(128);

                    b.Property<string>("Title")
                        .HasMaxLength(128);

                    b.Property<DateTime?>("UpdatedDate");

                    b.Property<string>("UpdatedUser")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.ToTable("Educators");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.EducatorSocialMedia", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("CreatedUser")
                        .HasMaxLength(128);

                    b.Property<string>("EducatorId");

                    b.Property<string>("Link");

                    b.Property<int>("SocialMediaType");

                    b.Property<DateTime?>("UpdatedDate");

                    b.Property<string>("UpdatedUser")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("EducatorId");

                    b.ToTable("EducatorSocialMedias");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.Sale", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("BillingType");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("CreatedUser")
                        .HasMaxLength(128);

                    b.Property<string>("TaxNo");

                    b.Property<string>("TaxOffice");

                    b.Property<DateTime?>("UpdatedDate");

                    b.Property<string>("UpdatedUser")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.ToTable("Sales");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.SaleAddress", b =>
                {
                    b.Property<Guid>("Id");

                    b.Property<string>("Address")
                        .HasMaxLength(256);

                    b.Property<string>("City")
                        .HasMaxLength(32);

                    b.Property<string>("County")
                        .HasMaxLength(32);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("CreatedUser")
                        .HasMaxLength(128);

                    b.Property<string>("PostalCode")
                        .HasMaxLength(8);

                    b.Property<DateTime?>("UpdatedDate");

                    b.Property<string>("UpdatedUser")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.ToTable("SaleAddresses");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.SaleDetail", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("CreatedUser")
                        .HasMaxLength(128);

                    b.Property<Guid>("EducationId");

                    b.Property<decimal>("Price");

                    b.Property<Guid?>("PromotionCodeId");

                    b.Property<Guid>("SaleId");

                    b.Property<DateTime?>("UpdatedDate");

                    b.Property<string>("UpdatedUser")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("EducationId");

                    b.HasIndex("PromotionCodeId");

                    b.HasIndex("SaleId");

                    b.ToTable("SaleDetails");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.StudentEducationInfo", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("CreatedUser")
                        .HasMaxLength(128);

                    b.Property<string>("CustomerId");

                    b.Property<int>("EducationCenter");

                    b.Property<DateTime>("StartedAt");

                    b.Property<DateTime?>("UpdatedDate");

                    b.Property<string>("UpdatedUser")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.ToTable("StudentEducationInfos");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.Suggestion", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CategoryId");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("CreatedUser")
                        .HasMaxLength(128);

                    b.Property<byte>("RangeMax");

                    b.Property<byte>("RangeMin");

                    b.Property<DateTime?>("UpdatedDate");

                    b.Property<string>("UpdatedUser")
                        .HasMaxLength(128);

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("SuggestionByCategory");
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
                    b.HasOne("NitelikliBilisim.Core.Entities.ApplicationRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("NitelikliBilisim.Core.Entities.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("NitelikliBilisim.Core.Entities.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("NitelikliBilisim.Core.Entities.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.ApplicationUserRole", b =>
                {
                    b.HasOne("NitelikliBilisim.Core.Entities.ApplicationRole", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("NitelikliBilisim.Core.Entities.ApplicationUser", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.Bridge_EducationTag", b =>
                {
                    b.HasOne("NitelikliBilisim.Core.Entities.EducationTag", "Tag")
                        .WithMany()
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("NitelikliBilisim.Core.Entities.Education", "Education")
                        .WithMany()
                        .HasForeignKey("Id2")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.Customer", b =>
                {
                    b.HasOne("NitelikliBilisim.Core.Entities.ApplicationUser", "User")
                        .WithMany()
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.Education", b =>
                {
                    b.HasOne("NitelikliBilisim.Core.Entities.EducationCategory", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.EducationCategory", b =>
                {
                    b.HasOne("NitelikliBilisim.Core.Entities.EducationCategory", "BaseCategory")
                        .WithMany()
                        .HasForeignKey("BaseCategoryId");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.EducationComment", b =>
                {
                    b.HasOne("NitelikliBilisim.Core.Entities.EducationComment", "BaseComment")
                        .WithMany()
                        .HasForeignKey("BaseCommentId");

                    b.HasOne("NitelikliBilisim.Core.Entities.Education", "Education")
                        .WithMany()
                        .HasForeignKey("EducationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.EducationGain", b =>
                {
                    b.HasOne("NitelikliBilisim.Core.Entities.Education", "Education")
                        .WithMany()
                        .HasForeignKey("EducationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.EducationMedia", b =>
                {
                    b.HasOne("NitelikliBilisim.Core.Entities.Education", "Education")
                        .WithMany()
                        .HasForeignKey("EducationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.EducationPart", b =>
                {
                    b.HasOne("NitelikliBilisim.Core.Entities.EducationPart", "BasePart")
                        .WithMany()
                        .HasForeignKey("BasePartId");

                    b.HasOne("NitelikliBilisim.Core.Entities.Education", "Education")
                        .WithMany()
                        .HasForeignKey("EducationId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.EducationTag", b =>
                {
                    b.HasOne("NitelikliBilisim.Core.Entities.EducationTag", "BaseTag")
                        .WithMany()
                        .HasForeignKey("BaseTagId");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.Educator", b =>
                {
                    b.HasOne("NitelikliBilisim.Core.Entities.ApplicationUser", "User")
                        .WithOne("Educator")
                        .HasForeignKey("NitelikliBilisim.Core.Entities.Educator", "Id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.EducatorSocialMedia", b =>
                {
                    b.HasOne("NitelikliBilisim.Core.Entities.Educator", "Educator")
                        .WithMany()
                        .HasForeignKey("EducatorId");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.SaleAddress", b =>
                {
                    b.HasOne("NitelikliBilisim.Core.Entities.Sale", "Sale")
                        .WithMany()
                        .HasForeignKey("Id")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.SaleDetail", b =>
                {
                    b.HasOne("NitelikliBilisim.Core.Entities.Education", "Education")
                        .WithMany()
                        .HasForeignKey("EducationId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("NitelikliBilisim.Core.Entities.EducationPromotionCode", "PromotionCode")
                        .WithMany()
                        .HasForeignKey("PromotionCodeId");

                    b.HasOne("NitelikliBilisim.Core.Entities.Sale", "Sale")
                        .WithMany()
                        .HasForeignKey("SaleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.StudentEducationInfo", b =>
                {
                    b.HasOne("NitelikliBilisim.Core.Entities.Customer", "Customer")
                        .WithMany()
                        .HasForeignKey("CustomerId");
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.Suggestion", b =>
                {
                    b.HasOne("NitelikliBilisim.Core.Entities.EducationCategory", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("NitelikliBilisim.Core.Entities.WishlistItem", b =>
                {
                    b.HasOne("NitelikliBilisim.Core.Entities.ApplicationUser", "User")
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
