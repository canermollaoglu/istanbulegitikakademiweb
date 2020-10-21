using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class EducationPromotionCodeTableAddValidityTypeandValidityValuesColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ValidityType",
                table: "EducationPromotionCodes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ValidityValues",
                table: "EducationPromotionCodes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ValidityType",
                table: "EducationPromotionCodes");

            migrationBuilder.DropColumn(
                name: "ValidityValues",
                table: "EducationPromotionCodes");
        }
    }
}
