using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class w : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceDetailsId",
                table: "Bridge_GroupStudents");

            migrationBuilder.AddColumn<Guid>(
                name: "TicketIdx",
                table: "Bridge_GroupStudents",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TicketId",
                table: "Bridge_GroupStudents");

            migrationBuilder.AddColumn<Guid>(
                name: "InvoiceDetailsId",
                table: "Bridge_GroupStudents",
                type: "uniqueidentifier",
                nullable: true);
        }
    }
}
