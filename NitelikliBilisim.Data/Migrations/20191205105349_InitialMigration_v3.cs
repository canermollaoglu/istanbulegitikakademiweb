using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class InitialMigration_v3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bridge_EducationCategories_EducationSpecialCategories_Id",
                table: "Bridge_EducationCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_EducationSpecialCategories_EducationSpecialCategories_BaseCategoryId",
                table: "EducationSpecialCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EducationSpecialCategories",
                table: "EducationSpecialCategories");

            migrationBuilder.RenameTable(
                name: "EducationSpecialCategories",
                newName: "EducationCategories");

            migrationBuilder.RenameColumn(
                name: "PhotoUrl",
                table: "EducationMediaItems",
                newName: "FileUrl");

            migrationBuilder.RenameIndex(
                name: "IX_EducationSpecialCategories_BaseCategoryId",
                table: "EducationCategories",
                newName: "IX_EducationCategories_BaseCategoryId");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Educations",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Level",
                table: "Educations",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EducationCategories",
                table: "EducationCategories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bridge_EducationCategories_EducationCategories_Id",
                table: "Bridge_EducationCategories",
                column: "Id",
                principalTable: "EducationCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EducationCategories_EducationCategories_BaseCategoryId",
                table: "EducationCategories",
                column: "BaseCategoryId",
                principalTable: "EducationCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bridge_EducationCategories_EducationCategories_Id",
                table: "Bridge_EducationCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_EducationCategories_EducationCategories_BaseCategoryId",
                table: "EducationCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EducationCategories",
                table: "EducationCategories");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Educations");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "Educations");

            migrationBuilder.RenameTable(
                name: "EducationCategories",
                newName: "EducationSpecialCategories");

            migrationBuilder.RenameColumn(
                name: "FileUrl",
                table: "EducationMediaItems",
                newName: "PhotoUrl");

            migrationBuilder.RenameIndex(
                name: "IX_EducationCategories_BaseCategoryId",
                table: "EducationSpecialCategories",
                newName: "IX_EducationSpecialCategories_BaseCategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EducationSpecialCategories",
                table: "EducationSpecialCategories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bridge_EducationCategories_EducationSpecialCategories_Id",
                table: "Bridge_EducationCategories",
                column: "Id",
                principalTable: "EducationSpecialCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_EducationSpecialCategories_EducationSpecialCategories_BaseCategoryId",
                table: "EducationSpecialCategories",
                column: "BaseCategoryId",
                principalTable: "EducationSpecialCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
