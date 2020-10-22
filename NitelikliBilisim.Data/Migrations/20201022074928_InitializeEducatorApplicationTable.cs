using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class InitializeEducatorApplicationTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EducatorApplications",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    NameSurname = table.Column<string>(maxLength: 100, nullable: false),
                    Phone = table.Column<string>(maxLength: 15, nullable: false),
                    Email = table.Column<string>(maxLength: 100, nullable: false),
                    Note = table.Column<string>(maxLength: 500, nullable: true),
                    CvUrl = table.Column<string>(nullable: true),
                    IsViewed = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducatorApplications", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EducatorApplications");
        }
    }
}
