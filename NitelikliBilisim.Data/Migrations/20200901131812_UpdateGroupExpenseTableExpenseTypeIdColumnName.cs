using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NitelikliBilisim.Data.Migrations
{
    public partial class UpdateGroupExpenseTableExpenseTypeIdColumnName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupExpenses_GroupExpenseTypes_ExpenseTypeId",
                table: "GroupExpenses");

            migrationBuilder.DropColumn(
                name: "GroupExpenseType",
                table: "GroupExpenses");

            migrationBuilder.AlterColumn<Guid>(
                name: "ExpenseTypeId",
                table: "GroupExpenses",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GroupExpenses_GroupExpenseTypes_ExpenseTypeId",
                table: "GroupExpenses",
                column: "ExpenseTypeId",
                principalTable: "GroupExpenseTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GroupExpenses_GroupExpenseTypes_ExpenseTypeId",
                table: "GroupExpenses");

            migrationBuilder.AlterColumn<Guid>(
                name: "ExpenseTypeId",
                table: "GroupExpenses",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid));

            migrationBuilder.AddColumn<Guid>(
                name: "GroupExpenseType",
                table: "GroupExpenses",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddForeignKey(
                name: "FK_GroupExpenses_GroupExpenseTypes_ExpenseTypeId",
                table: "GroupExpenses",
                column: "ExpenseTypeId",
                principalTable: "GroupExpenseTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
