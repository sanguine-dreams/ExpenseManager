using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseManager.Migrations
{
    /// <inheritdoc />
    public partial class AddGuildId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GuildId",
                table: "Expenses",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GuildId",
                table: "Accounts",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuildId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "GuildId",
                table: "Accounts");
        }
    }
}
