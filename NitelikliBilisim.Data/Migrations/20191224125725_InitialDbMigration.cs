using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class InitialDbMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bridge_EducationTags_Educations_Id",
                table: "Bridge_EducationTags");

            migrationBuilder.DropForeignKey(
                name: "FK_Bridge_EducationTags_EducationTags_Id2",
                table: "Bridge_EducationTags");

            migrationBuilder.AddForeignKey(
                name: "FK_Bridge_EducationTags_EducationTags_Id",
                table: "Bridge_EducationTags",
                column: "Id",
                principalTable: "EducationTags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bridge_EducationTags_Educations_Id2",
                table: "Bridge_EducationTags",
                column: "Id2",
                principalTable: "Educations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bridge_EducationTags_EducationTags_Id",
                table: "Bridge_EducationTags");

            migrationBuilder.DropForeignKey(
                name: "FK_Bridge_EducationTags_Educations_Id2",
                table: "Bridge_EducationTags");

            migrationBuilder.AddForeignKey(
                name: "FK_Bridge_EducationTags_Educations_Id",
                table: "Bridge_EducationTags",
                column: "Id",
                principalTable: "Educations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bridge_EducationTags_EducationTags_Id2",
                table: "Bridge_EducationTags",
                column: "Id2",
                principalTable: "EducationTags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
