using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class StudentEducationInfoUpdateCategoryColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "CategoryId",
                table: "StudentEducationInfos",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentEducationInfos_CategoryId",
                table: "StudentEducationInfos",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentEducationInfos_EducationCategories_CategoryId",
                table: "StudentEducationInfos",
                column: "CategoryId",
                principalTable: "EducationCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentEducationInfos_EducationCategories_CategoryId",
                table: "StudentEducationInfos");

            migrationBuilder.DropIndex(
                name: "IX_StudentEducationInfos_CategoryId",
                table: "StudentEducationInfos");

            migrationBuilder.AlterColumn<Guid>(
                name: "CategoryId",
                table: "StudentEducationInfos",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid));
        }
    }
}
