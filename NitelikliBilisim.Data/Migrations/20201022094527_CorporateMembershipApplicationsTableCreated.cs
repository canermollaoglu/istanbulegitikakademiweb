using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class CorporateMembershipApplicationsTableCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CorporateMembershipApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    CompanyName = table.Column<string>(maxLength: 150, nullable: false),
                    CompanySector = table.Column<string>(maxLength: 100, nullable: false),
                    NameSurname = table.Column<string>(maxLength: 100, nullable: false),
                    Phone = table.Column<string>(maxLength: 15, nullable: false),
                    Department = table.Column<string>(maxLength: 100, nullable: false),
                    NumberOfEmployees = table.Column<int>(nullable: false),
                    Address = table.Column<string>(nullable: true),
                    RequestNote = table.Column<string>(nullable: true),
                    IsViewed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CorporateMembershipApplications", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CorporateMembershipApplications");
        }
    }
}
