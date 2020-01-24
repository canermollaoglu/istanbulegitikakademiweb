using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class Add_Bridge_EducationEducator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bridge_EducationEducators",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Id2 = table.Column<string>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_Bridge_EducationEducators_Id2",
                table: "Bridge_EducationEducators",
                column: "Id2");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bridge_EducationEducators");
        }
    }
}
