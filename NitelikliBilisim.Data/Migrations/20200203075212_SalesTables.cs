using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class SalesTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Earning",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "TotalPaid",
                table: "Invoices");

            migrationBuilder.AlterColumn<string>(
                name: "TaxOffice",
                table: "Invoices",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TaxNo",
                table: "Invoices",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "Invoices",
                maxLength: 128,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    IsUsed = table.Column<bool>(nullable: false),
                    EducationId = table.Column<Guid>(nullable: false),
                    OwnerId = table.Column<string>(maxLength: 450, nullable: true),
                    HostId = table.Column<Guid>(nullable: false),
                    InvoiceDetailsId = table.Column<Guid>(nullable: false),
                    GroupId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tickets_Educations_EducationId",
                        column: x => x.EducationId,
                        principalTable: "Educations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tickets_EducationHosts_HostId",
                        column: x => x.HostId,
                        principalTable: "EducationHosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tickets_Customers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_EducationId",
                table: "Tickets",
                column: "EducationId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_HostId",
                table: "Tickets",
                column: "HostId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_OwnerId",
                table: "Tickets",
                column: "OwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "Invoices");

            migrationBuilder.AlterColumn<string>(
                name: "TaxOffice",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TaxNo",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Earning",
                table: "Invoices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPaid",
                table: "Invoices",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
