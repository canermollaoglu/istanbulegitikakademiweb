using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class RemoveValidityTypeAndValueColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValidityType",
                table: "EducationPromotionCodes");

            migrationBuilder.DropColumn(
                name: "ValidityValues",
                table: "EducationPromotionCodes");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ValidityType",
                table: "EducationPromotionCodes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ValidityValues",
                table: "EducationPromotionCodes",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
