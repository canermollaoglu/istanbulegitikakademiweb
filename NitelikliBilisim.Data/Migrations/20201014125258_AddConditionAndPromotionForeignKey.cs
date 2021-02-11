using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class AddConditionAndPromotionForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EducationPromotionCodeId",
                table: "EducationPromotionConditions",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_EducationPromotionConditions_EducationPromotionCodeId",
                table: "EducationPromotionConditions",
                column: "EducationPromotionCodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_EducationPromotionConditions_EducationPromotionCodes_EducationPromotionCodeId",
                table: "EducationPromotionConditions",
                column: "EducationPromotionCodeId",
                principalTable: "EducationPromotionCodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducationPromotionConditions_EducationPromotionCodes_EducationPromotionCodeId",
                table: "EducationPromotionConditions");

            migrationBuilder.DropIndex(
                name: "IX_EducationPromotionConditions_EducationPromotionCodeId",
                table: "EducationPromotionConditions");

            migrationBuilder.DropColumn(
                name: "EducationPromotionCodeId",
                table: "EducationPromotionConditions");
        }
    }
}
