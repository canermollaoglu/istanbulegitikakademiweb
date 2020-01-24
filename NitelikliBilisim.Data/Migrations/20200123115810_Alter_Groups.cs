using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class Alter_Groups : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsGroupOpenForAssignment",
                table: "EducationGroups",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte>(
                name: "Quota",
                table: "EducationGroups",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.CreateTable(
                name: "GroupLessonDays",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    DaysJson = table.Column<string>(maxLength: 128, nullable: true),
                    GroupId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupLessonDays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupLessonDays_EducationGroups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "EducationGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupLessonDays_GroupId",
                table: "GroupLessonDays",
                column: "GroupId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupLessonDays");

            migrationBuilder.DropColumn(
                name: "IsGroupOpenForAssignment",
                table: "EducationGroups");

            migrationBuilder.DropColumn(
                name: "Quota",
                table: "EducationGroups");
        }
    }
}
