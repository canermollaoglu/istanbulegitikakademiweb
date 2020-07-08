using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class updateOffDayTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "OffDays");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "OffDays");

            migrationBuilder.AddColumn<int>(
                name: "Day",
                table: "OffDays",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "OffDays",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "OffDays",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "OffDays",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Day",
                table: "OffDays");

            migrationBuilder.DropColumn(
                name: "Month",
                table: "OffDays");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "OffDays");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "OffDays");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "OffDays",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "OffDays",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
