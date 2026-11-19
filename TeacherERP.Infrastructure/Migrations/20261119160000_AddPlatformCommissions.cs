using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeacherERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPlatformCommissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlatformCommissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceTransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SubscriptionYear = table.Column<int>(type: "int", nullable: true),
                    SubscriptionMonth = table.Column<int>(type: "int", nullable: true),
                    SettledAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SettledByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SettlementReference = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlatformCommissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlatformCommissions_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlatformCommissions_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlatformCommissions_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlatformCommissions_Transactions_SourceTransactionId",
                        column: x => x.SourceTransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlatformCommissions_GroupId",
                table: "PlatformCommissions",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformCommissions_SourceTransactionId",
                table: "PlatformCommissions",
                column: "SourceTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformCommissions_StudentId",
                table: "PlatformCommissions",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformCommissions_TenantId_CreatedAt_SettledAtUtc",
                table: "PlatformCommissions",
                columns: new[] { "TenantId", "CreatedAt", "SettledAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_PlatformCommissions_TenantId_StudentId_GroupId_SubscriptionYear_SubscriptionMonth_Type",
                table: "PlatformCommissions",
                columns: new[] { "TenantId", "StudentId", "GroupId", "SubscriptionYear", "SubscriptionMonth", "Type" },
                unique: true,
                filter: "[SubscriptionYear] IS NOT NULL AND [SubscriptionMonth] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformCommissions_TenantId_StudentId_SessionId_Type",
                table: "PlatformCommissions",
                columns: new[] { "TenantId", "StudentId", "SessionId", "Type" },
                unique: true,
                filter: "[SessionId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlatformCommissions");
        }
    }
}
