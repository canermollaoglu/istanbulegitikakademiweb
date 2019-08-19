using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class NewHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DataHistories",
                table: "DataHistories");

            migrationBuilder.RenameTable(
                name: "DataHistories",
                newName: "AutoHistory");

            migrationBuilder.AlterColumn<string>(
                name: "TableName",
                table: "AutoHistory",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RowId",
                table: "AutoHistory",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AlterColumn<string>(
                name: "TableName",
                table: "DataHistories",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "RowId",
                table: "DataHistories",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AddPrimaryKey(
                name: "PK_DataHistories",
                table: "DataHistories",
                column: "Id");
        }
    }
}
