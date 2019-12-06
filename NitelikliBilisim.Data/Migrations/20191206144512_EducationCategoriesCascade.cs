using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class EducationCategoriesCascade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducationCategories_EducationCategories_BaseCategoryId",
                table: "EducationCategories");

            migrationBuilder.AddForeignKey(
                name: "FK_EducationCategories_EducationCategories_BaseCategoryId",
                table: "EducationCategories",
                column: "BaseCategoryId",
                principalTable: "EducationCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducationCategories_EducationCategories_BaseCategoryId",
                table: "EducationCategories");

            migrationBuilder.AddForeignKey(
                name: "FK_EducationCategories_EducationCategories_BaseCategoryId",
                table: "EducationCategories",
                column: "BaseCategoryId",
                principalTable: "EducationCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
