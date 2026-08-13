using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineAccountingApp.Persistence.Migrations.AppDb
{
    /// <inheritdoc />
    public partial class mig_8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MainRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRoleCreateByAdmin = table.Column<bool>(type: "bit", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MainRoles_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MainRoleAndRoleRelationships",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MainRoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainRoleAndRoleRelationships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MainRoleAndRoleRelationships_MainRoles_MainRoleId",
                        column: x => x.MainRoleId,
                        principalTable: "MainRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MainRoleAndRoleRelationships_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MainRoleAndUserRelationships",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MainRoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainRoleAndUserRelationships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MainRoleAndUserRelationships_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MainRoleAndUserRelationships_MainRoles_MainRoleId",
                        column: x => x.MainRoleId,
                        principalTable: "MainRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MainRoleAndUserRelationships_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MainRoleAndRoleRelationships_MainRoleId",
                table: "MainRoleAndRoleRelationships",
                column: "MainRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_MainRoleAndRoleRelationships_RoleId",
                table: "MainRoleAndRoleRelationships",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_MainRoleAndUserRelationships_CompanyId",
                table: "MainRoleAndUserRelationships",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_MainRoleAndUserRelationships_MainRoleId",
                table: "MainRoleAndUserRelationships",
                column: "MainRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_MainRoleAndUserRelationships_UserId",
                table: "MainRoleAndUserRelationships",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MainRoles_CompanyId",
                table: "MainRoles",
                column: "CompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MainRoleAndRoleRelationships");

            migrationBuilder.DropTable(
                name: "MainRoleAndUserRelationships");

            migrationBuilder.DropTable(
                name: "MainRoles");
        }
    }
}
