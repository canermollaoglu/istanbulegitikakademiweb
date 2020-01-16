using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class SaleDetailsRemove : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SaleDetails");

            migrationBuilder.AddColumn<string>(
                name: "CustomerId",
                table: "Sales",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Earning",
                table: "Sales",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<Guid>(
                name: "EducationId",
                table: "Sales",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsCash",
                table: "Sales",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "Paid",
                table: "Sales",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<byte>(
                name: "PaymentCount",
                table: "Sales",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceAtCurrentDate",
                table: "Sales",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "EducationGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    GroupName = table.Column<string>(nullable: true),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EducatorId = table.Column<string>(maxLength: 128, nullable: true),
                    EducationId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducationGroups_Educations_EducationId",
                        column: x => x.EducationId,
                        principalTable: "Educations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Bridge_GroupStudents",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Id2 = table.Column<string>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    SaleId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bridge_GroupStudents", x => new { x.Id, x.Id2 });
                    table.ForeignKey(
                        name: "FK_Bridge_GroupStudents_EducationGroups_Id",
                        column: x => x.Id,
                        principalTable: "EducationGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Bridge_GroupStudents_Customers_Id2",
                        column: x => x.Id2,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sales_CustomerId",
                table: "Sales",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Bridge_GroupStudents_Id2",
                table: "Bridge_GroupStudents",
                column: "Id2");

            migrationBuilder.CreateIndex(
                name: "IX_EducationGroups_EducationId",
                table: "EducationGroups",
                column: "EducationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Customers_CustomerId",
                table: "Sales",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Customers_CustomerId",
                table: "Sales");

            migrationBuilder.DropTable(
                name: "Bridge_GroupStudents");

            migrationBuilder.DropTable(
                name: "EducationGroups");

            migrationBuilder.DropIndex(
                name: "IX_Sales_CustomerId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "Earning",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "EducationId",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "IsCash",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "Paid",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "PaymentCount",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "PriceAtCurrentDate",
                table: "Sales");

            migrationBuilder.CreateTable(
                name: "SaleDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    EducationId = table.Column<Guid>(nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    PromotionCodeId = table.Column<Guid>(nullable: true),
                    SaleId = table.Column<Guid>(nullable: false),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleDetails_Educations_EducationId",
                        column: x => x.EducationId,
                        principalTable: "Educations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SaleDetails_EducationPromotionCodes_PromotionCodeId",
                        column: x => x.PromotionCodeId,
                        principalTable: "EducationPromotionCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SaleDetails_Sales_SaleId",
                        column: x => x.SaleId,
                        principalTable: "Sales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SaleDetails_EducationId",
                table: "SaleDetails",
                column: "EducationId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleDetails_PromotionCodeId",
                table: "SaleDetails",
                column: "PromotionCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_SaleDetails_SaleId",
                table: "SaleDetails",
                column: "SaleId");
        }
    }
}
