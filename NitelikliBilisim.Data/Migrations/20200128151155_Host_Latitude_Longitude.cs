using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class Host_Latitude_Longitude : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Latitude",
                table: "EducationHosts",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Longitude",
                table: "EducationHosts",
                maxLength: 128,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "EducationHosts");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "EducationHosts");
        }
    }
}
