using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class TempSaleData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConversationId",
                table: "OnlinePaymentInfos");

            migrationBuilder.CreateTable(
                name: "Temp_UserSaleData",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 450, nullable: false),
                    Data = table.Column<string>(maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Temp_UserSaleData", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Temp_UserSaleData");

            migrationBuilder.AddColumn<Guid>(
                name: "ConversationId",
                table: "OnlinePaymentInfos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
