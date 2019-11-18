using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Educator_AspNetUsers_Id",
                table: "Educator");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Educator",
                table: "Educator");

            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "Educator");

            migrationBuilder.RenameTable(
                name: "Educator",
                newName: "Educators");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Educators",
                table: "Educators",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    CustomerType = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 128, nullable: true),
                    Surname = table.Column<string>(maxLength: 32, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Customers_AspNetUsers_Id",
                        column: x => x.Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EducationComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Content = table.Column<string>(maxLength: 512, nullable: true),
                    Points = table.Column<byte>(nullable: false),
                    IsEducatorComment = table.Column<bool>(nullable: false),
                    CommentatorId = table.Column<string>(maxLength: 128, nullable: true),
                    ApproverId = table.Column<string>(maxLength: 128, nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    BaseCommentId = table.Column<Guid>(nullable: true),
                    EducationId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducationComments_EducationComments_BaseCommentId",
                        column: x => x.BaseCommentId,
                        principalTable: "EducationComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EducationComments_Educations_EducationId",
                        column: x => x.EducationId,
                        principalTable: "Educations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EducationMediaItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    PhotoUrl = table.Column<string>(maxLength: 256, nullable: true),
                    MediaType = table.Column<int>(nullable: false),
                    EducationId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationMediaItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducationMediaItems_Educations_EducationId",
                        column: x => x.EducationId,
                        principalTable: "Educations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EducationParts",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Title = table.Column<string>(maxLength: 128, nullable: true),
                    Duration = table.Column<byte>(nullable: false),
                    Order = table.Column<byte>(nullable: false),
                    EducationId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationParts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducationParts_Educations_EducationId",
                        column: x => x.EducationId,
                        principalTable: "Educations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EducationPromotionCodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    ValidThru = table.Column<DateTime>(nullable: false),
                    Code = table.Column<string>(maxLength: 7, nullable: true),
                    OffPercentage = table.Column<byte>(nullable: false),
                    IsUsed = table.Column<bool>(nullable: false),
                    TimesUsable = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationPromotionCodes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sales",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    BillingType = table.Column<int>(nullable: false),
                    TaxNo = table.Column<string>(nullable: true),
                    TaxOffice = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SaleAddresses",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    City = table.Column<string>(maxLength: 32, nullable: true),
                    County = table.Column<string>(maxLength: 32, nullable: true),
                    Address = table.Column<string>(maxLength: 256, nullable: true),
                    PostalCode = table.Column<string>(maxLength: 8, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SaleAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SaleAddresses_Sales_Id",
                        column: x => x.Id,
                        principalTable: "Sales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SaleDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    Price = table.Column<decimal>(nullable: false),
                    SaleId = table.Column<Guid>(nullable: false),
                    EducationId = table.Column<Guid>(nullable: false),
                    PromotionCodeId = table.Column<Guid>(nullable: true)
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
                name: "IX_EducationComments_BaseCommentId",
                table: "EducationComments",
                column: "BaseCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationComments_EducationId",
                table: "EducationComments",
                column: "EducationId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationMediaItems_EducationId",
                table: "EducationMediaItems",
                column: "EducationId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationParts_EducationId",
                table: "EducationParts",
                column: "EducationId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Educators_AspNetUsers_Id",
                table: "Educators",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Educators_AspNetUsers_Id",
                table: "Educators");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "EducationComments");

            migrationBuilder.DropTable(
                name: "EducationMediaItems");

            migrationBuilder.DropTable(
                name: "EducationParts");

            migrationBuilder.DropTable(
                name: "SaleAddresses");

            migrationBuilder.DropTable(
                name: "SaleDetails");

            migrationBuilder.DropTable(
                name: "EducationPromotionCodes");

            migrationBuilder.DropTable(
                name: "Sales");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Educators",
                table: "Educators");

            migrationBuilder.RenameTable(
                name: "Educators",
                newName: "Educator");

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "Educator",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Educator",
                table: "Educator",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Educator_AspNetUsers_Id",
                table: "Educator",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
