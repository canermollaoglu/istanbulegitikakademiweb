using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class SuggestionTableDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SuggestionByCategory");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SuggestionByCategory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    RangeMax = table.Column<byte>(type: "tinyint", nullable: false),
                    RangeMin = table.Column<byte>(type: "tinyint", nullable: false),
                    SuggestableEducations = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedUser = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuggestionByCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SuggestionByCategory_EducationCategories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "EducationCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SuggestionByCategory_CategoryId",
                table: "SuggestionByCategory",
                column: "CategoryId");
        }
    }
}
