using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class Satis : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Satislar",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Il = table.Column<string>(nullable: true),
                    Ilce = table.Column<string>(nullable: true),
                    PostaKodu = table.Column<string>(nullable: true),
                    FaturaTipi = table.Column<int>(nullable: false),
                    FirmaAdi = table.Column<string>(nullable: true),
                    VergiNumarasi = table.Column<string>(nullable: true),
                    VergiDairesi = table.Column<string>(nullable: true),
                    EFaturaMukellefiMi = table.Column<bool>(nullable: false),
                    Adres = table.Column<string>(nullable: true),
                    UserIp = table.Column<string>(maxLength: 16, nullable: true),
                    KartBilgi = table.Column<string>(maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Satislar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SatisDetaylar",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Id2 = table.Column<Guid>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Fiyat = table.Column<decimal>(type: "decimal(8, 2)", nullable: false),
                    Indirim = table.Column<decimal>(type: "decimal(3, 3)", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_SatisDetaylar_Id2",
                table: "SatisDetaylar",
                column: "Id2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SatisDetaylar");

            migrationBuilder.DropTable(
                name: "Satislar");
        }
    }
}
