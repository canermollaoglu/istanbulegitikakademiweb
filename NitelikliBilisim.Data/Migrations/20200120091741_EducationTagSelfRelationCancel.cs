using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class EducationTagSelfRelationCancel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducationTags_EducationTags_BaseTagId",
                table: "EducationTags");

            migrationBuilder.DropIndex(
                name: "IX_EducationTags_BaseTagId",
                table: "EducationTags");

            migrationBuilder.DropColumn(
                name: "BaseTagId",
                table: "EducationTags");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BaseTagId",
                table: "EducationTags",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EducationTags_BaseTagId",
                table: "EducationTags",
                column: "BaseTagId");

            migrationBuilder.AddForeignKey(
                name: "FK_EducationTags_EducationTags_BaseTagId",
                table: "EducationTags",
                column: "BaseTagId",
                principalTable: "EducationTags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
