using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class AddColumnBlockage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BlockageResolveDate",
                table: "OnlinePaymentDetailsInfos",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "GroupLessonDays",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    GroupId = table.Column<Guid>(nullable: false),
                    DateOfLesson = table.Column<DateTime>(nullable: false),
                    HasAttendanceRecord = table.Column<bool>(nullable: false),
                    IsImmuneToAutoChange = table.Column<bool>(nullable: false)
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
                name: "BlockageResolveDate",
                table: "OnlinePaymentDetailsInfos");
        }
    }
}
