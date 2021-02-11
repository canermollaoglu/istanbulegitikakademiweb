using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class OnlinePaymentInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConversationId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "IsCash",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "PaymentId",
                table: "Invoices");

            migrationBuilder.AddColumn<int>(
                name: "TransactionStatus",
                table: "Invoices",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "OnlinePaymentDetailsInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    TransactionId = table.Column<string>(maxLength: 16, nullable: true),
                    Price = table.Column<decimal>(nullable: false),
                    CommissionFee = table.Column<decimal>(nullable: false),
                    CommisionRate = table.Column<decimal>(nullable: false),
                    MerchantPayout = table.Column<decimal>(nullable: false),
                    PaidPrice = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnlinePaymentDetailsInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OnlinePaymentDetailsInfos_InvoiceDetails_Id",
                        column: x => x.Id,
                        principalTable: "InvoiceDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OnlinePaymentInfos",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CreatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedUser = table.Column<string>(maxLength: 128, nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    ConversationId = table.Column<Guid>(nullable: false),
                    PaymentId = table.Column<string>(maxLength: 16, nullable: true),
                    BinNumber = table.Column<string>(maxLength: 6, nullable: true),
                    LastFourDigit = table.Column<string>(maxLength: 4, nullable: true),
                    HostRef = table.Column<string>(maxLength: 16, nullable: true),
                    CommissonFee = table.Column<decimal>(nullable: false),
                    CommissionRate = table.Column<decimal>(nullable: false),
                    PaidPrice = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OnlinePaymentInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OnlinePaymentInfos_Invoices_Id",
                        column: x => x.Id,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OnlinePaymentDetailsInfos");

            migrationBuilder.DropTable(
                name: "OnlinePaymentInfos");

            migrationBuilder.DropColumn(
                name: "TransactionStatus",
                table: "Invoices");

            migrationBuilder.AddColumn<Guid>(
                name: "ConversationId",
                table: "Invoices",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsCash",
                table: "Invoices",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PaymentId",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
