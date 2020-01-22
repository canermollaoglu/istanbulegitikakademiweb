using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class Added_EducationHosts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "GroupName",
                table: "EducationGroups",
                maxLength: 128,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "HostId",
                table: "EducationGroups",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "EducationHosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    City = table.Column<int>(nullable: false),
                    Address = table.Column<string>(maxLength: 2048, nullable: true),
                    HostName = table.Column<string>(maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationHosts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EducationGroups_HostId",
                table: "EducationGroups",
                column: "HostId");

            migrationBuilder.AddForeignKey(
                name: "FK_EducationGroups_EducationHosts_HostId",
                table: "EducationGroups",
                column: "HostId",
                principalTable: "EducationHosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EducationGroups_EducationHosts_HostId",
                table: "EducationGroups");

            migrationBuilder.DropTable(
                name: "EducationHosts");

            migrationBuilder.DropIndex(
                name: "IX_EducationGroups_HostId",
                table: "EducationGroups");

            migrationBuilder.DropColumn(
                name: "HostId",
                table: "EducationGroups");

            migrationBuilder.AlterColumn<string>(
                name: "GroupName",
                table: "EducationGroups",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 128,
                oldNullable: true);
        }
    }
}
