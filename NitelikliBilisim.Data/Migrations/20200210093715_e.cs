using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class e : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TicketIdx1",
                table: "Bridge_GroupStudents",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
            migrationBuilder.DropColumn(
                name: "TicketIdx",
                table: "Bridge_GroupStudents");
            migrationBuilder.DropColumn(
                name: "TicketId",
                table: "Bridge_GroupStudents");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TicketIdx1",
                table: "Bridge_GroupStudents");
            migrationBuilder.AddColumn<Guid>(
                name: "TicketIdx",
                table: "Bridge_GroupStudents",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
            migrationBuilder.AddColumn<Guid>(
                name: "TicketId",
                table: "Bridge_GroupStudents",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
