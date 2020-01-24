using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class AddSuggestion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Surname",
                table: "Customers");

            migrationBuilder.AddColumn<bool>(
                name: "IsNbuyStudent",
                table: "Customers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "StudentEducationInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    EducationCenter = table.Column<int>(nullable: false),
                    StartedAt = table.Column<DateTime>(nullable: false),
                    CustomerId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentEducationInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentEducationInfos_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentEducationInfos_CustomerId",
                table: "StudentEducationInfos",
                column: "CustomerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentEducationInfos");

            migrationBuilder.DropColumn(
                name: "IsNbuyStudent",
                table: "Customers");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Customers",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Surname",
                table: "Customers",
                maxLength: 32,
                nullable: true);
        }
    }
}
