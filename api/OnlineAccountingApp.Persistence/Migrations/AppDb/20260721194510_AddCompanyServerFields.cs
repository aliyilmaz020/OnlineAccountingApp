using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineAccountingApp.Persistence.Migrations.AppDb
{
    /// <inheritdoc />
    public partial class AddCompanyServerFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DatabaseName",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ServerName",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ServerPassword",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ServerUserId",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DatabaseName",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "ServerName",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "ServerPassword",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "ServerUserId",
                table: "Companies");
        }
    }
}
