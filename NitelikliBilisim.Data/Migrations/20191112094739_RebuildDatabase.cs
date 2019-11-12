using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class RebuildDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Egiticiler");

            migrationBuilder.DropTable(
                name: "EgitimDetaylari");

            migrationBuilder.DropTable(
                name: "EgitimKategoriler");

            migrationBuilder.DropTable(
                name: "EgitimKazanimlar");

            migrationBuilder.DropTable(
                name: "MusteriYorumlar");

            migrationBuilder.DropTable(
                name: "SatisDetaylar");

            migrationBuilder.DropTable(
                name: "Sepetler");

            migrationBuilder.DropTable(
                name: "Kategoriler");

            migrationBuilder.DropTable(
                name: "Egitimler");

            migrationBuilder.DropTable(
                name: "Satislar");

            migrationBuilder.DropColumn(
                name: "FotoUrl",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Surname",
                table: "AspNetUsers",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AvatarPath",
                table: "AspNetUsers",
                maxLength: 256,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Educations",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 128, nullable: true),
                    Description = table.Column<string>(maxLength: 512, nullable: true),
                    NewPrice = table.Column<decimal>(type: "decimal(8, 2)", nullable: true),
                    OldPrice = table.Column<decimal>(type: "decimal(8, 2)", nullable: true),
                    Days = table.Column<byte>(nullable: false),
                    HoursPerDay = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Educations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EducationSpecialCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 128, nullable: true),
                    Description = table.Column<string>(maxLength: 512, nullable: true),
                    BaseCategoryId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationSpecialCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducationSpecialCategories_EducationSpecialCategories_BaseCategoryId",
                        column: x => x.BaseCategoryId,
                        principalTable: "EducationSpecialCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Educator",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Title = table.Column<string>(maxLength: 128, nullable: true),
                    Biography = table.Column<string>(maxLength: 8192, nullable: true),
                    PhotoUrl = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Educator", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Educator_AspNetUsers_Id",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Wishlist",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Id2 = table.Column<Guid>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true)
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
                name: "Bridge_EducationCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Id2 = table.Column<Guid>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bridge_EducationCategories", x => new { x.Id, x.Id2 });
                    table.ForeignKey(
                        name: "FK_Bridge_EducationCategories_EducationSpecialCategories_Id",
                        column: x => x.Id,
                        principalTable: "EducationSpecialCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bridge_EducationCategories_Educations_Id2",
                        column: x => x.Id2,
                        principalTable: "Educations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bridge_EducationCategories_Id2",
                table: "Bridge_EducationCategories",
                column: "Id2");

            migrationBuilder.CreateIndex(
                name: "IX_EducationSpecialCategories_BaseCategoryId",
                table: "EducationSpecialCategories",
                column: "BaseCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Wishlist_Id2",
                table: "Wishlist",
                column: "Id2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bridge_EducationCategories");

            migrationBuilder.DropTable(
                name: "Educator");

            migrationBuilder.DropTable(
                name: "Wishlist");

            migrationBuilder.DropTable(
                name: "EducationSpecialCategories");

            migrationBuilder.DropTable(
                name: "Educations");

            migrationBuilder.DropColumn(
                name: "AvatarPath",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Surname",
                table: "AspNetUsers",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUsers",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 128,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FotoUrl",
                table: "AspNetUsers",
                maxLength: 250,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Egiticiler",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 450, nullable: false),
                    Biyografi = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    DetayFoto = table.Column<string>(maxLength: 250, nullable: true),
                    Title = table.Column<string>(maxLength: 120, nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Egiticiler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Egiticiler_AspNetUsers_Id",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Egitimler",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Aciklama = table.Column<string>(nullable: false),
                    Ad = table.Column<string>(maxLength: 150, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    DetayFoto = table.Column<string>(maxLength: 250, nullable: true),
                    EgitimFoto = table.Column<string>(maxLength: 250, nullable: true),
                    EskiFiyat = table.Column<decimal>(type: "decimal(8, 2)", nullable: true),
                    Fiyat = table.Column<decimal>(type: "decimal(8, 2)", nullable: false),
                    GunSayisi = table.Column<int>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    VideoUrl = table.Column<string>(maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Egitimler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kategoriler",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Aciklama = table.Column<string>(maxLength: 500, nullable: true),
                    Ad = table.Column<string>(maxLength: 150, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    DetayFoto = table.Column<string>(maxLength: 250, nullable: true),
                    KategoriFoto = table.Column<string>(maxLength: 250, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    VideoUrl = table.Column<string>(maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kategoriler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Satislar",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Adres = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    EFaturaMukellefiMi = table.Column<bool>(nullable: false),
                    FaturaTipi = table.Column<int>(nullable: false),
                    FirmaAdi = table.Column<string>(nullable: true),
                    Il = table.Column<string>(nullable: true),
                    Ilce = table.Column<string>(nullable: true),
                    KartBilgi = table.Column<string>(maxLength: 10, nullable: true),
                    PostaKodu = table.Column<string>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    UserIp = table.Column<string>(maxLength: 16, nullable: true),
                    VergiDairesi = table.Column<string>(nullable: true),
                    VergiNumarasi = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Satislar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EgitimDetaylari",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Baslik = table.Column<string>(maxLength: 120, nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    EgitimId = table.Column<Guid>(nullable: false),
                    Konu = table.Column<string>(nullable: true),
                    Sira = table.Column<int>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EgitimDetaylari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EgitimDetaylari_Egitimler_EgitimId",
                        column: x => x.EgitimId,
                        principalTable: "Egitimler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EgitimKazanimlar",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    EgitimId = table.Column<Guid>(nullable: false),
                    KazanimAdi = table.Column<string>(nullable: true),
                    Simge = table.Column<string>(nullable: true),
                    Sira = table.Column<int>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EgitimKazanimlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EgitimKazanimlar_Egitimler_EgitimId",
                        column: x => x.EgitimId,
                        principalTable: "Egitimler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MusteriYorumlar",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    EgitimId = table.Column<Guid>(nullable: false),
                    KullaniciId = table.Column<string>(maxLength: 128, nullable: false),
                    OnaylanmaTarihi = table.Column<DateTime>(nullable: true),
                    OnaylayanId = table.Column<string>(maxLength: 128, nullable: true),
                    Puan = table.Column<int>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    Yorum = table.Column<string>(maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MusteriYorumlar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MusteriYorumlar_Egitimler_EgitimId",
                        column: x => x.EgitimId,
                        principalTable: "Egitimler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EgitimKategoriler",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Id2 = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EgitimKategoriler", x => new { x.Id, x.Id2 });
                    table.ForeignKey(
                        name: "FK_EgitimKategoriler_Egitimler_Id",
                        column: x => x.Id,
                        principalTable: "Egitimler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EgitimKategoriler_Kategoriler_Id2",
                        column: x => x.Id2,
                        principalTable: "Kategoriler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SatisDetaylar",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Id2 = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    Fiyat = table.Column<decimal>(type: "decimal(8, 2)", nullable: false),
                    Indirim = table.Column<decimal>(type: "decimal(3, 3)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SatisDetaylar", x => new { x.Id, x.Id2 });
                    table.ForeignKey(
                        name: "FK_SatisDetaylar_Satislar_Id",
                        column: x => x.Id,
                        principalTable: "Satislar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SatisDetaylar_Egitimler_Id2",
                        column: x => x.Id2,
                        principalTable: "Egitimler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sepetler",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    EgitimId = table.Column<Guid>(nullable: false),
                    SatisId = table.Column<Guid>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sepetler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sepetler_Egitimler_EgitimId",
                        column: x => x.EgitimId,
                        principalTable: "Egitimler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sepetler_Satislar_SatisId",
                        column: x => x.SatisId,
                        principalTable: "Satislar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EgitimDetaylari_EgitimId",
                table: "EgitimDetaylari",
                column: "EgitimId");

            migrationBuilder.CreateIndex(
                name: "IX_EgitimKategoriler_Id2",
                table: "EgitimKategoriler",
                column: "Id2");

            migrationBuilder.CreateIndex(
                name: "IX_EgitimKazanimlar_EgitimId",
                table: "EgitimKazanimlar",
                column: "EgitimId");

            migrationBuilder.CreateIndex(
                name: "IX_MusteriYorumlar_EgitimId",
                table: "MusteriYorumlar",
                column: "EgitimId");

            migrationBuilder.CreateIndex(
                name: "IX_SatisDetaylar_Id2",
                table: "SatisDetaylar",
                column: "Id2");

            migrationBuilder.CreateIndex(
                name: "IX_Sepetler_EgitimId",
                table: "Sepetler",
                column: "EgitimId");

            migrationBuilder.CreateIndex(
                name: "IX_Sepetler_SatisId",
                table: "Sepetler",
                column: "SatisId");
        }
    }
}
