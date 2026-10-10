using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseManager.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AccountId",
                table: "Expenses",
                type: "TEXT",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE Expenses
                SET AccountId = (
                    SELECT a.Id
                    FROM Accounts a
                    WHERE a.GuildId = Expenses.GuildId
                      AND a.Nickname = Expenses.PurchasedBy
                    ORDER BY a.Id
                    LIMIT 1
                )
                WHERE AccountId IS NULL
                   OR AccountId = '00000000-0000-0000-0000-000000000000'
                   OR NOT EXISTS (
                       SELECT 1
                       FROM Accounts a
                       WHERE a.Id = Expenses.AccountId
                   );
            ");

            migrationBuilder.Sql(@"
                DELETE FROM Expenses
                WHERE AccountId IS NULL
                   OR AccountId = '00000000-0000-0000-0000-000000000000'
                   OR NOT EXISTS (
                       SELECT 1
                       FROM Accounts a
                       WHERE a.Id = Expenses.AccountId
                   );
            ");

            migrationBuilder.AlterColumn<Guid>(
                name: "AccountId",
                table: "Expenses",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_AccountId",
                table: "Expenses",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_Accounts_AccountId",
                table: "Expenses",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_Accounts_AccountId",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_AccountId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Expenses");
        }
    }
}
