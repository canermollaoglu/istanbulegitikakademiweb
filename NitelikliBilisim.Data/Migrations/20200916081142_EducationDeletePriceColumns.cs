using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class EducationDeletePriceColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewPrice",
                table: "Educations");

            migrationBuilder.DropColumn(
                name: "OldPrice",
                table: "Educations");

            migrationBuilder.AddColumn<Guid>(
                name: "GroupId",
                table: "InvoiceDetails",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "InvoiceDetails");

            migrationBuilder.AddColumn<decimal>(
                name: "NewPrice",
                table: "Educations",
                type: "decimal(8, 2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OldPrice",
                table: "Educations",
                type: "decimal(8, 2)",
                nullable: true);
        }
    }
}
