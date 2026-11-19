using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeacherERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInstaPayReceiptWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ReceiptImageUrl",
                table: "Transactions",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PaymentContextSessionId",
                table: "Transactions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiptImagePublicId",
                table: "Transactions",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TeacherPaymentSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InstaPayLink = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    QrImageUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false),
                    QrImagePublicId = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherPaymentSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TeacherPaymentSettings_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TenantId_StudentId_Type_PaymentContextSessionId",
                table: "Transactions",
                columns: new[] { "TenantId", "StudentId", "Type", "PaymentContextSessionId" },
                unique: true,
                filter: "[Status] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherPaymentSettings_TenantId",
                table: "TeacherPaymentSettings",
                column: "TenantId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeacherPaymentSettings");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_TenantId_StudentId_Type_PaymentContextSessionId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "PaymentContextSessionId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "ReceiptImagePublicId",
                table: "Transactions");

            migrationBuilder.AlterColumn<string>(
                name: "ReceiptImageUrl",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2048)",
                oldMaxLength: 2048,
                oldNullable: true);
        }
    }
}
