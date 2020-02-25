using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class AddEducatorId1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EducatorId",
                table: "GroupLessonDays",
                maxLength: 128,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EducatorId",
                table: "GroupLessonDays");
        }
    }
}
