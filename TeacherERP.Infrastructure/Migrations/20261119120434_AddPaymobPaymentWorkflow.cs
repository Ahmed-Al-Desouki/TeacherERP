using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeacherERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymobPaymentWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transactions_TenantId_StudentId_Type_PaymentContextSessionId",
                table: "Transactions");

            migrationBuilder.AddColumn<string>(
                name: "IdempotencyKey",
                table: "Transactions",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Provider",
                table: "Transactions",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProviderClientSecret",
                table: "Transactions",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProviderFailureCode",
                table: "Transactions",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProviderIntegrationId",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProviderIntentionId",
                table: "Transactions",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProviderOrderId",
                table: "Transactions",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProviderRequestStartedAtUtc",
                table: "Transactions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestFingerprint",
                table: "Transactions",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PaymentEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ProviderEventId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    EventType = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    PreviousStatus = table.Column<int>(type: "int", nullable: true),
                    CurrentStatus = table.Column<int>(type: "int", nullable: true),
                    ProviderOrderId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ProviderOccurredAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PayloadSha256 = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentEvents_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentEvents_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_Provider_ProviderOrderId",
                table: "Transactions",
                columns: new[] { "Provider", "ProviderOrderId" });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TenantId_StudentId_IdempotencyKey",
                table: "Transactions",
                columns: new[] { "TenantId", "StudentId", "IdempotencyKey" },
                unique: true,
                filter: "[IdempotencyKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TenantId_StudentId_Type_PaymentContextSessionId",
                table: "Transactions",
                columns: new[] { "TenantId", "StudentId", "Type", "PaymentContextSessionId" },
                unique: true,
                filter: "[Status] IN (0, 4, 5)");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentEvents_Provider_ProviderEventId",
                table: "PaymentEvents",
                columns: new[] { "Provider", "ProviderEventId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentEvents_TenantId_TransactionId_CreatedAt",
                table: "PaymentEvents",
                columns: new[] { "TenantId", "TransactionId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentEvents_TransactionId",
                table: "PaymentEvents",
                column: "TransactionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentEvents");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_Provider_ProviderOrderId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_TenantId_StudentId_IdempotencyKey",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_TenantId_StudentId_Type_PaymentContextSessionId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "IdempotencyKey",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "Provider",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ProviderClientSecret",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ProviderFailureCode",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ProviderIntegrationId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ProviderIntentionId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ProviderOrderId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ProviderRequestStartedAtUtc",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "RequestFingerprint",
                table: "Transactions");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TenantId_StudentId_Type_PaymentContextSessionId",
                table: "Transactions",
                columns: new[] { "TenantId", "StudentId", "Type", "PaymentContextSessionId" },
                unique: true,
                filter: "[Status] = 0");
        }
    }
}
