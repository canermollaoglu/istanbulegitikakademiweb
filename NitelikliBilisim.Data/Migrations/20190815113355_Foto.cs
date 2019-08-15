using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class Foto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BannerFoto",
                table: "Kategoriler",
                newName: "KategoriFoto");

            migrationBuilder.RenameColumn(
                name: "BannerFoto",
                table: "Egitimler",
                newName: "EgitimFoto");

            migrationBuilder.AddColumn<string>(
                name: "DetayFoto",
                table: "Egiticiler",
                maxLength: 250,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DetayFoto",
                table: "Egiticiler");

            migrationBuilder.RenameColumn(
                name: "KategoriFoto",
                table: "Kategoriler",
                newName: "BannerFoto");

            migrationBuilder.RenameColumn(
                name: "EgitimFoto",
                table: "Egitimler",
                newName: "BannerFoto");
        }
    }
}
