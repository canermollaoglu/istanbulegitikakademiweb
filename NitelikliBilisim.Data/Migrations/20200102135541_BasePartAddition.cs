using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class BasePartAddition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BasePartId",
                table: "EducationParts",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "EducationGains",
                maxLength: 128,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EducationParts_BasePartId",
                table: "EducationParts",
                column: "BasePartId");

            migrationBuilder.AddForeignKey(
                name: "FK_EducationParts_EducationParts_BasePartId",
                table: "EducationParts",
                column: "BasePartId",
                principalTable: "EducationParts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducationParts_EducationParts_BasePartId",
                table: "EducationParts");

            migrationBuilder.DropIndex(
                name: "IX_EducationParts_BasePartId",
                table: "EducationParts");

            migrationBuilder.DropColumn(
                name: "BasePartId",
                table: "EducationParts");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "EducationGains");
        }
    }
}
