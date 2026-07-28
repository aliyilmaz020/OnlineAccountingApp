using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineAccountingApp.Persistence.Migrations.AppDb
{
    /// <inheritdoc />
    public partial class mig_7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "EditDate",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateDate",
                table: "Roles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Roles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "EditDate",
                table: "Roles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Roles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "EditDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "CreateDate",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "EditDate",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Roles");
        }
    }
}
