using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CurrencyExchangeManager.Api.Migrations
{
    /// <inheritdoc />
    public partial class removeExtraColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Base",
                table: "CurrencyHistory");

            migrationBuilder.RenameColumn(
                name: "Target",
                table: "CurrencyHistory",
                newName: "ExchangeRate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExchangeRate",
                table: "CurrencyHistory",
                newName: "Target");

            migrationBuilder.AddColumn<string>(
                name: "Base",
                table: "CurrencyHistory",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
