using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class DeleteColumnEducatorPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EducatorPrice",
                table: "EducationGroups");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "EducatorPrice",
                table: "EducationGroups",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
