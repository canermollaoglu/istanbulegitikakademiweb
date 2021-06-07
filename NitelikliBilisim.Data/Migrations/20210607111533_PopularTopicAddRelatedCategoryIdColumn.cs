using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class PopularTopicAddRelatedCategoryIdColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RelatedCategoryId",
                table: "PopularTopics",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_PopularTopics_RelatedCategoryId",
                table: "PopularTopics",
                column: "RelatedCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_PopularTopics_EducationCategories_RelatedCategoryId",
                table: "PopularTopics",
                column: "RelatedCategoryId",
                principalTable: "EducationCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PopularTopics_EducationCategories_RelatedCategoryId",
                table: "PopularTopics");

            migrationBuilder.DropIndex(
                name: "IX_PopularTopics_RelatedCategoryId",
                table: "PopularTopics");

            migrationBuilder.DropColumn(
                name: "RelatedCategoryId",
                table: "PopularTopics");
        }
    }
}
