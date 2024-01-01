using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WaseetAPI.Database.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "companies",
                columns: table => new
                {
                    company_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    server_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    server_user_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    server_password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    db_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_own_database = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_companies", x => x.company_id);
                });

            migrationBuilder.CreateTable(
                name: "logs",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    msg_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    msg = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    msg_type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_logs", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sallamerchantapp",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    merchant_id = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    app_name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sallamerchantapp", x => new { x.id, x.merchant_id, x.app_name });
                });

            migrationBuilder.CreateTable(
                name: "sallawebhookrequests",
                columns: table => new
                {
                    id = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    salla_user_id = table.Column<int>(type: "int", nullable: false),
                    event_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    event_id = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sallawebhookrequests", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "admins",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    company_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    user_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    user_image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    user_address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    activation_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    logged = table.Column<bool>(type: "bit", nullable: true),
                    logged_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    is_admin = table.Column<bool>(type: "bit", nullable: true),
                    user_online_type = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_admins", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_admins_companies_company_id",
                        column: x => x.company_id,
                        principalTable: "companies",
                        principalColumn: "company_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "sallausers",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    company_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    user_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    activation_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    user_online_type = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sallausers", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_sallausers_companies_company_id",
                        column: x => x.company_id,
                        principalTable: "companies",
                        principalColumn: "company_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    company_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    user_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    user_image = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    user_address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    activation_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    logged = table.Column<bool>(type: "bit", nullable: true),
                    logged_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    is_admin = table.Column<bool>(type: "bit", nullable: true),
                    user_online_type = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_users_companies_company_id",
                        column: x => x.company_id,
                        principalTable: "companies",
                        principalColumn: "company_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_admins_company_id",
                table: "admins",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_sallausers_company_id",
                table: "sallausers",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_company_id",
                table: "users",
                column: "company_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admins");

            migrationBuilder.DropTable(
                name: "logs");

            migrationBuilder.DropTable(
                name: "sallamerchantapp");

            migrationBuilder.DropTable(
                name: "sallausers");

            migrationBuilder.DropTable(
                name: "sallawebhookrequests");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "companies");
        }
    }
}
