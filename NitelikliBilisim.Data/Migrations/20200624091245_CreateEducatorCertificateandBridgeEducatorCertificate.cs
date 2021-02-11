using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class CreateEducatorCertificateandBridgeEducatorCertificate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EducatorCertificates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    CertificateImagePath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducatorCertificates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bridge_EducatorCertificates",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Id2 = table.Column<int>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bridge_EducatorCertificates", x => new { x.Id, x.Id2 });
                    table.ForeignKey(
                        name: "FK_Bridge_EducatorCertificates_Educators_Id",
                        column: x => x.Id,
                        principalTable: "Educators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bridge_EducatorCertificates_EducatorCertificates_Id2",
                        column: x => x.Id2,
                        principalTable: "EducatorCertificates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bridge_EducatorCertificates_Id2",
                table: "Bridge_EducatorCertificates",
                column: "Id2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bridge_EducatorCertificates");

            migrationBuilder.DropTable(
                name: "EducatorCertificates");
        }
    }
}
