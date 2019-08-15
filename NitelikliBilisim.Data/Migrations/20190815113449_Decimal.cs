using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class Decimal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Fiyat",
                table: "Egitimler",
                type: "decimal(8, 2)",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<decimal>(
                name: "EskiFiyat",
                table: "Egitimler",
                type: "decimal(8, 2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Fiyat",
                table: "Egitimler",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(8, 2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "EskiFiyat",
                table: "Egitimler",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(8, 2)",
                oldNullable: true);
        }
    }
}
