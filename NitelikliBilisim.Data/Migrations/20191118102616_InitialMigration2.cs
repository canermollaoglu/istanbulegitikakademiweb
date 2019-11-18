using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class InitialMigration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EducatorSocialMedias",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    EducatorId = table.Column<string>(nullable: true),
                    SocialMediaType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducatorSocialMedias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducatorSocialMedias_Educators_EducatorId",
                        column: x => x.EducatorId,
                        principalTable: "Educators",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EducatorSocialMedias_EducatorId",
                table: "EducatorSocialMedias",
                column: "EducatorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EducatorSocialMedias");
        }
    }
}
