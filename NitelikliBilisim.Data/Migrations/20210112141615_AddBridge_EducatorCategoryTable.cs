using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class AddBridge_EducatorCategoryTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_Bridge_EducatorCategories_Id2",
                table: "Bridge_EducatorCategories",
                column: "Id2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bridge_EducatorCategories");
        }
    }
}
