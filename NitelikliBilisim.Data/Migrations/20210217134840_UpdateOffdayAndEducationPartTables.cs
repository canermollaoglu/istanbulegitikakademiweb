using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class UpdateOffdayAndEducationPartTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Duration",
                table: "EducationParts");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "OffDays",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedUser",
                table: "OffDays",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "OffDays",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedUser",
                table: "OffDays",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "OffDays");

            migrationBuilder.DropColumn(
                name: "CreatedUser",
                table: "OffDays");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "OffDays");

            migrationBuilder.DropColumn(
                name: "UpdatedUser",
                table: "OffDays");

            migrationBuilder.AddColumn<byte>(
                name: "Duration",
                table: "EducationParts",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);
        }
    }
}
