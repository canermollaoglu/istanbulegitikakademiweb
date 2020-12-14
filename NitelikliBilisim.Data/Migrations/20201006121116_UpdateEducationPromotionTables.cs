using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class UpdateEducationPromotionTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "EducationPromotionCodes");

            migrationBuilder.DropColumn(
                name: "IsUsed",
                table: "EducationPromotionCodes");

            migrationBuilder.DropColumn(
                name: "OffPercentage",
                table: "EducationPromotionCodes");

            migrationBuilder.DropColumn(
                name: "TimesUsable",
                table: "EducationPromotionCodes");

            migrationBuilder.DropColumn(
                name: "ValidThru",
                table: "EducationPromotionCodes");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "EducationPromotionCodes",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "EducationPromotionCodes",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "EducationPromotionCodes",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "MaxUsageLimit",
                table: "EducationPromotionCodes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "MinBasketAmount",
                table: "EducationPromotionCodes",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PromotionCode",
                table: "EducationPromotionCodes",
                maxLength: 7,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "EducationPromotionCodes",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "UserBasedUsageLimit",
                table: "EducationPromotionCodes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EducationPromotionItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    InvoiceId = table.Column<Guid>(nullable: false),
                    EducationPromotionCodeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationPromotionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducationPromotionItems_EducationPromotionCodes_EducationPromotionCodeId",
                        column: x => x.EducationPromotionCodeId,
                        principalTable: "EducationPromotionCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EducationPromotionItems_EducationPromotionCodeId",
                table: "EducationPromotionItems",
                column: "EducationPromotionCodeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EducationPromotionItems");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "EducationPromotionCodes");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "EducationPromotionCodes");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "EducationPromotionCodes");

            migrationBuilder.DropColumn(
                name: "MaxUsageLimit",
                table: "EducationPromotionCodes");

            migrationBuilder.DropColumn(
                name: "MinBasketAmount",
                table: "EducationPromotionCodes");

            migrationBuilder.DropColumn(
                name: "PromotionCode",
                table: "EducationPromotionCodes");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "EducationPromotionCodes");

            migrationBuilder.DropColumn(
                name: "UserBasedUsageLimit",
                table: "EducationPromotionCodes");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "EducationPromotionCodes",
                type: "nvarchar(7)",
                maxLength: 7,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                table: "EducationPromotionCodes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte>(
                name: "OffPercentage",
                table: "EducationPromotionCodes",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<byte>(
                name: "TimesUsable",
                table: "EducationPromotionCodes",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ValidThru",
                table: "EducationPromotionCodes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
