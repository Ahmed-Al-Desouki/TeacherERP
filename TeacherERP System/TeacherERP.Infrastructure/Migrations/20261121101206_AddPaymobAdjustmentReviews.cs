using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeacherERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymobAdjustmentReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ProviderRefundedAmount",
                table: "Transactions",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "PaymobAdjustmentReviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProviderEventId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    ProviderAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    StudentDebtAdjustmentAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CommissionReversalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReviewedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DecisionNote = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymobAdjustmentReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymobAdjustmentReviews_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymobAdjustmentReviews_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymobAdjustmentReviews_ProviderEventId",
                table: "PaymobAdjustmentReviews",
                column: "ProviderEventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymobAdjustmentReviews_Status_CreatedAt_Id",
                table: "PaymobAdjustmentReviews",
                columns: new[] { "Status", "CreatedAt", "Id" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymobAdjustmentReviews_TenantId_TransactionId",
                table: "PaymobAdjustmentReviews",
                columns: new[] { "TenantId", "TransactionId" });

            migrationBuilder.CreateIndex(
                name: "IX_PaymobAdjustmentReviews_TransactionId",
                table: "PaymobAdjustmentReviews",
                column: "TransactionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymobAdjustmentReviews");

            migrationBuilder.DropColumn(
                name: "ProviderRefundedAmount",
                table: "Transactions");
        }
    }
}
