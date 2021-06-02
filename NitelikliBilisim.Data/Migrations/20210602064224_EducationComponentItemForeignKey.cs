using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class EducationComponentItemForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_EducationComponentItems_EducationId",
                table: "EducationComponentItems",
                column: "EducationId");

            migrationBuilder.AddForeignKey(
                name: "FK_EducationComponentItems_Educations_EducationId",
                table: "EducationComponentItems",
                column: "EducationId",
                principalTable: "Educations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducationComponentItems_Educations_EducationId",
                table: "EducationComponentItems");

            migrationBuilder.DropIndex(
                name: "IX_EducationComponentItems_EducationId",
                table: "EducationComponentItems");
        }
    }
}
