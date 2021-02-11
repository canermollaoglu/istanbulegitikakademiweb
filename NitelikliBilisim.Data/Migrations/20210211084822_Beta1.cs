using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class Beta1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Surname = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    AvatarPath = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BannerAds",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IconUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Title2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RelatedApplicationUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BannerAds", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlogCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    SeoUrl = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlogSubscribers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsCanceled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogSubscribers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlogTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContactForms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactFormType = table.Column<int>(type: "int", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactForms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CorporateMembershipApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CompanySector = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameSurname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NumberOfEmployees = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsViewed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorporateMembershipApplications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RowId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TableName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Changed = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kind = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EducationCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Description2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CategoryType = table.Column<int>(type: "int", nullable: false),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    SeoUrl = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IconUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BackgroundImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WizardClass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IconColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EducationDayCount = table.Column<int>(type: "int", nullable: true),
                    BaseCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducationCategories_EducationCategories_BaseCategoryId",
                        column: x => x.BaseCategoryId,
                        principalTable: "EducationCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EducationHosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    City = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    HostName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Latitude = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Longitude = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    GoogleMapUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationHosts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EducationPromotionCodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PromotionType = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PromotionCode = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaxUsageLimit = table.Column<int>(type: "int", nullable: false),
                    UserBasedUsageLimit = table.Column<int>(type: "int", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MinBasketAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationPromotionCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EducationTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EducatorApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NameSurname = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CvUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsViewed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducatorApplications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EducatorCertificates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CertificateImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducatorCertificates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EducatorSalaries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Paid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EducatorId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    EarnedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EarnedForGroup = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducatorSalaries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FeaturedComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreviewImageFileUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeaturedComments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupExpenseTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupExpenseTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NewsletterSubscribers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsletterSubscribers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OffDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Day = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OffDays", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Temp_UserSaleData",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Data = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Temp_UserSaleData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Universities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Universities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CustomerType = table.Column<int>(type: "int", nullable: false),
                    IsNbuyStudent = table.Column<bool>(type: "bit", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastGraduatedSchoolId = table.Column<int>(type: "int", nullable: true),
                    Job = table.Column<int>(type: "int", nullable: false),
                    CityId = table.Column<int>(type: "int", nullable: true),
                    WebSiteUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LinkedInProfileUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customers_AspNetUsers_Id",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Educators",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Biography = table.Column<string>(type: "nvarchar(max)", maxLength: 8192, nullable: true),
                    ShortDescription = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    Bank = table.Column<int>(type: "int", nullable: false),
                    IBAN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Educators", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Educators_AspNetUsers_Id",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlogPosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SummaryContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FeaturedImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReadingTime = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SeoUrl = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    IsHighLight = table.Column<bool>(type: "bit", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlogPosts_BlogCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "BlogCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "States",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CityId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.Id);
                    table.ForeignKey(
                        name: "FK_States_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Educations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Description2 = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Days = table.Column<byte>(type: "tinyint", nullable: false),
                    HoursPerDay = table.Column<byte>(type: "tinyint", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    SeoUrl = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    VideoUrl = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    IsFeaturedEducation = table.Column<bool>(type: "bit", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Educations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Educations_EducationCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "EducationCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Classrooms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    HostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classrooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Classrooms_EducationHosts_HostId",
                        column: x => x.HostId,
                        principalTable: "EducationHosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EducationHostImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    EducationHostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationHostImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducationHostImages_EducationHosts_EducationHostId",
                        column: x => x.EducationHostId,
                        principalTable: "EducationHosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EducationPromotionConditions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConditionType = table.Column<int>(type: "int", nullable: false),
                    ConditionValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EducationPromotionCodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationPromotionConditions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducationPromotionConditions_EducationPromotionCodes_EducationPromotionCodeId",
                        column: x => x.EducationPromotionCodeId,
                        principalTable: "EducationPromotionCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EducationPromotionItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    InvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EducationPromotionCodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationPromotionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducationPromotionItems_EducationPromotionCodes_EducationPromotionCodeId",
                        column: x => x.EducationPromotionCodeId,
                        principalTable: "EducationPromotionCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CustomerCertificates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerCertificates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerCertificates_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Invoices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BillingType = table.Column<int>(type: "int", nullable: false),
                    TransactionStatus = table.Column<int>(type: "int", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    TaxNo = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    TaxOffice = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    PaymentCount = table.Column<byte>(type: "tinyint", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    CreatedIp = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    UpdatedIp = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    InvoicePdfUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invoices_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentEducationInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EducationCenter = table.Column<int>(type: "int", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentEducationInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentEducationInfos_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentEducationInfos_EducationCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "EducationCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bridge_EducatorCategories",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id2 = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bridge_EducatorCategories", x => new { x.Id, x.Id2 });
                    table.ForeignKey(
                        name: "FK_Bridge_EducatorCategories_EducationCategories_Id2",
                        column: x => x.Id2,
                        principalTable: "EducationCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bridge_EducatorCategories_Educators_Id",
                        column: x => x.Id,
                        principalTable: "Educators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bridge_EducatorCertificates",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id2 = table.Column<int>(type: "int", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bridge_EducatorCertificates", x => new { x.Id, x.Id2 });
                    table.ForeignKey(
                        name: "FK_Bridge_EducatorCertificates_EducatorCertificates_Id2",
                        column: x => x.Id2,
                        principalTable: "EducatorCertificates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bridge_EducatorCertificates_Educators_Id",
                        column: x => x.Id,
                        principalTable: "Educators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EducatorSocialMedias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EducatorId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    SocialMediaType = table.Column<int>(type: "int", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducatorSocialMedias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducatorSocialMedias_Educators_EducatorId",
                        column: x => x.EducatorId,
                        principalTable: "Educators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bridge_BlogTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id2 = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bridge_BlogTags", x => new { x.Id, x.Id2 });
                    table.ForeignKey(
                        name: "FK_Bridge_BlogTags_BlogPosts_Id",
                        column: x => x.Id,
                        principalTable: "BlogPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bridge_BlogTags_BlogTags_Id2",
                        column: x => x.Id2,
                        principalTable: "BlogTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDefaultAddress = table.Column<bool>(type: "bit", nullable: false),
                    NameSurname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxOffice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdentityNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StateId = table.Column<int>(type: "int", nullable: true),
                    CityId = table.Column<int>(type: "int", nullable: true),
                    AddressType = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Addresses_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Addresses_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Addresses_States_StateId",
                        column: x => x.StateId,
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Bridge_EducationEducators",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id2 = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bridge_EducationEducators", x => new { x.Id, x.Id2 });
                    table.ForeignKey(
                        name: "FK_Bridge_EducationEducators_Educations_Id",
                        column: x => x.Id,
                        principalTable: "Educations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bridge_EducationEducators_Educators_Id2",
                        column: x => x.Id2,
                        principalTable: "Educators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bridge_EducationTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id2 = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bridge_EducationTags", x => new { x.Id, x.Id2 });
                    table.ForeignKey(
                        name: "FK_Bridge_EducationTags_Educations_Id2",
                        column: x => x.Id2,
                        principalTable: "Educations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bridge_EducationTags_EducationTags_Id",
                        column: x => x.Id,
                        principalTable: "EducationTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EducationComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Points = table.Column<byte>(type: "tinyint", nullable: false),
                    IsEducatorComment = table.Column<bool>(type: "bit", nullable: false),
                    CommentatorId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ApproverId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    IsHighLight = table.Column<bool>(type: "bit", nullable: false),
                    ApprovalStatus = table.Column<int>(type: "int", nullable: false),
                    BaseCommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EducationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducationComments_EducationComments_BaseCommentId",
                        column: x => x.BaseCommentId,
                        principalTable: "EducationComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EducationComments_Educations_EducationId",
                        column: x => x.EducationId,
                        principalTable: "Educations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EducationGains",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Gain = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    EducationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationGains", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducationGains_Educations_EducationId",
                        column: x => x.EducationId,
                        principalTable: "Educations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EducationGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EducatorId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    EducationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsGroupOpenForAssignment = table.Column<bool>(type: "bit", nullable: false),
                    Quota = table.Column<byte>(type: "tinyint", nullable: false),
                    ExtraPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    NewPrice = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    OldPrice = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducationGroups_EducationHosts_HostId",
                        column: x => x.HostId,
                        principalTable: "EducationHosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EducationGroups_Educations_EducationId",
                        column: x => x.EducationId,
                        principalTable: "Educations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EducationMediaItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    MediaType = table.Column<int>(type: "int", nullable: false),
                    EducationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationMediaItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducationMediaItems_Educations_EducationId",
                        column: x => x.EducationId,
                        principalTable: "Educations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EducationParts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    Duration = table.Column<byte>(type: "tinyint", nullable: false),
                    Order = table.Column<byte>(type: "tinyint", nullable: false),
                    EducationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BasePartId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationParts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducationParts_EducationParts_BasePartId",
                        column: x => x.BasePartId,
                        principalTable: "EducationParts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EducationParts_Educations_EducationId",
                        column: x => x.EducationId,
                        principalTable: "Educations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EducationSuggestionCriterions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CriterionType = table.Column<int>(type: "int", nullable: false),
                    MinValue = table.Column<int>(type: "int", nullable: true),
                    MaxValue = table.Column<int>(type: "int", nullable: true),
                    CharValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EducationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationSuggestionCriterions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducationSuggestionCriterions_Educations_EducationId",
                        column: x => x.EducationId,
                        principalTable: "Educations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    EducationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    HostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceDetailsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickets_Customers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_EducationHosts_HostId",
                        column: x => x.HostId,
                        principalTable: "EducationHosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tickets_Educations_EducationId",
                        column: x => x.EducationId,
                        principalTable: "Educations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wishlist",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id2 = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wishlist", x => new { x.Id, x.Id2 });
                    table.ForeignKey(
                        name: "FK_Wishlist_AspNetUsers_Id",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Wishlist_Educations_Id2",
                        column: x => x.Id2,
                        principalTable: "Educations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceAddresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    City = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    County = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceAddresses_Invoices_Id",
                        column: x => x.Id,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EducationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PriceAtCurrentDate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsUsedAsTicket = table.Column<bool>(type: "bit", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InvoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceDetails_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OnlinePaymentInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PaymentId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    BinNumber = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true),
                    LastFourDigit = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: true),
                    HostRef = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    CommissonFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CommissionRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaidPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnlinePaymentInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OnlinePaymentInfos_Invoices_Id",
                        column: x => x.Id,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EducationDays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Day = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StudentEducationInfoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationDays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducationDays_StudentEducationInfos_StudentEducationInfoId",
                        column: x => x.StudentEducationInfoId,
                        principalTable: "StudentEducationInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bridge_GroupStudents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id2 = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TicketId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bridge_GroupStudents", x => new { x.Id, x.Id2 });
                    table.ForeignKey(
                        name: "FK_Bridge_GroupStudents_Customers_Id2",
                        column: x => x.Id2,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bridge_GroupStudents_EducationGroups_Id",
                        column: x => x.Id,
                        principalTable: "EducationGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupAttendances",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupAttendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupAttendances_EducationGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "EducationGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupExpenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Count = table.Column<byte>(type: "tinyint", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExpenseTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupExpenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupExpenses_EducationGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "EducationGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupExpenses_GroupExpenseTypes_ExpenseTypeId",
                        column: x => x.ExpenseTypeId,
                        principalTable: "GroupExpenseTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupLessonDays",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DateOfLesson = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HasAttendanceRecord = table.Column<bool>(type: "bit", nullable: false),
                    IsImmuneToAutoChange = table.Column<bool>(type: "bit", nullable: false),
                    EducatorId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ClassroomId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EducatorSalary = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupLessonDays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupLessonDays_EducationGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "EducationGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WeekDaysOfGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DaysJson = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeekDaysOfGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeekDaysOfGroups_EducationGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "EducationGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OnlinePaymentDetailsInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CommissionFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CommisionRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MerchantPayout = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaidPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BlockageResolveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCancelled = table.Column<bool>(type: "bit", nullable: false),
                    CancellationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RefundPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnlinePaymentDetailsInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OnlinePaymentDetailsInfos_InvoiceDetails_Id",
                        column: x => x.Id,
                        principalTable: "InvoiceDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CityId",
                table: "Addresses",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_CustomerId",
                table: "Addresses",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_StateId",
                table: "Addresses",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_CategoryId",
                table: "BlogPosts",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Bridge_BlogTags_Id2",
                table: "Bridge_BlogTags",
                column: "Id2");

            migrationBuilder.CreateIndex(
                name: "IX_Bridge_EducationEducators_Id2",
                table: "Bridge_EducationEducators",
                column: "Id2");

            migrationBuilder.CreateIndex(
                name: "IX_Bridge_EducationTags_Id2",
                table: "Bridge_EducationTags",
                column: "Id2");

            migrationBuilder.CreateIndex(
                name: "IX_Bridge_EducatorCategories_Id2",
                table: "Bridge_EducatorCategories",
                column: "Id2");

            migrationBuilder.CreateIndex(
                name: "IX_Bridge_EducatorCertificates_Id2",
                table: "Bridge_EducatorCertificates",
                column: "Id2");

            migrationBuilder.CreateIndex(
                name: "IX_Bridge_GroupStudents_Id2",
                table: "Bridge_GroupStudents",
                column: "Id2");

            migrationBuilder.CreateIndex(
                name: "IX_Classrooms_HostId",
                table: "Classrooms",
                column: "HostId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerCertificates_CustomerId",
                table: "CustomerCertificates",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationCategories_BaseCategoryId",
                table: "EducationCategories",
                column: "BaseCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationComments_BaseCommentId",
                table: "EducationComments",
                column: "BaseCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationComments_EducationId",
                table: "EducationComments",
                column: "EducationId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationDays_StudentEducationInfoId",
                table: "EducationDays",
                column: "StudentEducationInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationGains_EducationId",
                table: "EducationGains",
                column: "EducationId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationGroups_EducationId",
                table: "EducationGroups",
                column: "EducationId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationGroups_HostId",
                table: "EducationGroups",
                column: "HostId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationHostImages_EducationHostId",
                table: "EducationHostImages",
                column: "EducationHostId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationMediaItems_EducationId",
                table: "EducationMediaItems",
                column: "EducationId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationParts_BasePartId",
                table: "EducationParts",
                column: "BasePartId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationParts_EducationId",
                table: "EducationParts",
                column: "EducationId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationPromotionConditions_EducationPromotionCodeId",
                table: "EducationPromotionConditions",
                column: "EducationPromotionCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationPromotionItems_EducationPromotionCodeId",
                table: "EducationPromotionItems",
                column: "EducationPromotionCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Educations_CategoryId",
                table: "Educations",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationSuggestionCriterions_EducationId",
                table: "EducationSuggestionCriterions",
                column: "EducationId");

            migrationBuilder.CreateIndex(
                name: "IX_EducatorSocialMedias_EducatorId",
                table: "EducatorSocialMedias",
                column: "EducatorId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupAttendances_GroupId",
                table: "GroupAttendances",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupExpenses_ExpenseTypeId",
                table: "GroupExpenses",
                column: "ExpenseTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupExpenses_GroupId",
                table: "GroupExpenses",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupLessonDays_GroupId",
                table: "GroupLessonDays",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceDetails_InvoiceId",
                table: "InvoiceDetails",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_CustomerId",
                table: "Invoices",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_States_CityId",
                table: "States",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentEducationInfos_CategoryId",
                table: "StudentEducationInfos",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentEducationInfos_CustomerId",
                table: "StudentEducationInfos",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_EducationId",
                table: "Tickets",
                column: "EducationId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_HostId",
                table: "Tickets",
                column: "HostId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_OwnerId",
                table: "Tickets",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_WeekDaysOfGroups_GroupId",
                table: "WeekDaysOfGroups",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Wishlist_Id2",
                table: "Wishlist",
                column: "Id2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BannerAds");

            migrationBuilder.DropTable(
                name: "BlogSubscribers");

            migrationBuilder.DropTable(
                name: "Bridge_BlogTags");

            migrationBuilder.DropTable(
                name: "Bridge_EducationEducators");

            migrationBuilder.DropTable(
                name: "Bridge_EducationTags");

            migrationBuilder.DropTable(
                name: "Bridge_EducatorCategories");

            migrationBuilder.DropTable(
                name: "Bridge_EducatorCertificates");

            migrationBuilder.DropTable(
                name: "Bridge_GroupStudents");

            migrationBuilder.DropTable(
                name: "Classrooms");

            migrationBuilder.DropTable(
                name: "ContactForms");

            migrationBuilder.DropTable(
                name: "CorporateMembershipApplications");

            migrationBuilder.DropTable(
                name: "CustomerCertificates");

            migrationBuilder.DropTable(
                name: "DataHistories");

            migrationBuilder.DropTable(
                name: "EducationComments");

            migrationBuilder.DropTable(
                name: "EducationDays");

            migrationBuilder.DropTable(
                name: "EducationGains");

            migrationBuilder.DropTable(
                name: "EducationHostImages");

            migrationBuilder.DropTable(
                name: "EducationMediaItems");

            migrationBuilder.DropTable(
                name: "EducationParts");

            migrationBuilder.DropTable(
                name: "EducationPromotionConditions");

            migrationBuilder.DropTable(
                name: "EducationPromotionItems");

            migrationBuilder.DropTable(
                name: "EducationSuggestionCriterions");

            migrationBuilder.DropTable(
                name: "EducatorApplications");

            migrationBuilder.DropTable(
                name: "EducatorSalaries");

            migrationBuilder.DropTable(
                name: "EducatorSocialMedias");

            migrationBuilder.DropTable(
                name: "FeaturedComments");

            migrationBuilder.DropTable(
                name: "GroupAttendances");

            migrationBuilder.DropTable(
                name: "GroupExpenses");

            migrationBuilder.DropTable(
                name: "GroupLessonDays");

            migrationBuilder.DropTable(
                name: "InvoiceAddresses");

            migrationBuilder.DropTable(
                name: "NewsletterSubscribers");

            migrationBuilder.DropTable(
                name: "OffDays");

            migrationBuilder.DropTable(
                name: "OnlinePaymentDetailsInfos");

            migrationBuilder.DropTable(
                name: "OnlinePaymentInfos");

            migrationBuilder.DropTable(
                name: "Temp_UserSaleData");

            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "Universities");

            migrationBuilder.DropTable(
                name: "WeekDaysOfGroups");

            migrationBuilder.DropTable(
                name: "Wishlist");

            migrationBuilder.DropTable(
                name: "States");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "BlogPosts");

            migrationBuilder.DropTable(
                name: "BlogTags");

            migrationBuilder.DropTable(
                name: "EducationTags");

            migrationBuilder.DropTable(
                name: "EducatorCertificates");

            migrationBuilder.DropTable(
                name: "StudentEducationInfos");

            migrationBuilder.DropTable(
                name: "EducationPromotionCodes");

            migrationBuilder.DropTable(
                name: "Educators");

            migrationBuilder.DropTable(
                name: "GroupExpenseTypes");

            migrationBuilder.DropTable(
                name: "InvoiceDetails");

            migrationBuilder.DropTable(
                name: "EducationGroups");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "BlogCategories");

            migrationBuilder.DropTable(
                name: "Invoices");

            migrationBuilder.DropTable(
                name: "EducationHosts");

            migrationBuilder.DropTable(
                name: "Educations");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "EducationCategories");

            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
