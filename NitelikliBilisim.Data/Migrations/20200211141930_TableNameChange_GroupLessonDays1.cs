using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class TableNameChange_GroupLessonDays1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WeekDaysOfGroup_EducationGroups_GroupId",
                table: "WeekDaysOfGroup");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WeekDaysOfGroup",
                table: "WeekDaysOfGroup");

            migrationBuilder.RenameTable(
                name: "WeekDaysOfGroup",
                newName: "WeekDaysOfGroups");

            migrationBuilder.RenameIndex(
                name: "IX_WeekDaysOfGroup_GroupId",
                table: "WeekDaysOfGroups",
                newName: "IX_WeekDaysOfGroups_GroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WeekDaysOfGroups",
                table: "WeekDaysOfGroups",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WeekDaysOfGroups_EducationGroups_GroupId",
                table: "WeekDaysOfGroups",
                column: "GroupId",
                principalTable: "EducationGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WeekDaysOfGroups_EducationGroups_GroupId",
                table: "WeekDaysOfGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WeekDaysOfGroups",
                table: "WeekDaysOfGroups");

            migrationBuilder.RenameTable(
                name: "WeekDaysOfGroups",
                newName: "WeekDaysOfGroup");

            migrationBuilder.RenameIndex(
                name: "IX_WeekDaysOfGroups_GroupId",
                table: "WeekDaysOfGroup",
                newName: "IX_WeekDaysOfGroup_GroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WeekDaysOfGroup",
                table: "WeekDaysOfGroup",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WeekDaysOfGroup_EducationGroups_GroupId",
                table: "WeekDaysOfGroup",
                column: "GroupId",
                principalTable: "EducationGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
