using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class AttendanceAndIp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedIp",
                table: "Invoices",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedIp",
                table: "Invoices",
                maxLength: 32,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GroupAttendances",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false),
                    CustomerId = table.Column<string>(maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupAttendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupAttendances_EducationGroups_Id",
                        column: x => x.Id,
                        principalTable: "EducationGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupAttendances");

            migrationBuilder.DropColumn(
                name: "CreatedIp",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "UpdatedIp",
                table: "Invoices");
        }
    }
}
