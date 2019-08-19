using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class NewHistory2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AutoHistory",
                table: "AutoHistory");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AutoHistory");

            migrationBuilder.RenameTable(
                name: "AutoHistory",
                newName: "DataHistories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DataHistories",
                table: "DataHistories",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DataHistories",
                table: "DataHistories");

            migrationBuilder.RenameTable(
                name: "DataHistories",
                newName: "AutoHistory");

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AutoHistory",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AutoHistory",
                table: "AutoHistory",
                column: "Id");
        }
    }
}
