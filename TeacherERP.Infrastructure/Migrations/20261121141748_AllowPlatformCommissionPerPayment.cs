using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeacherERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AllowPlatformCommissionPerPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlatformCommissions_TenantId_StudentId_SessionId_Type",
                table: "PlatformCommissions");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformCommissions_TenantId_StudentId_SessionId_Type_SourceTransactionId",
                table: "PlatformCommissions",
                columns: new[] { "TenantId", "StudentId", "SessionId", "Type", "SourceTransactionId" },
                unique: true,
                filter: "[SessionId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PlatformCommissions_TenantId_StudentId_SessionId_Type_SourceTransactionId",
                table: "PlatformCommissions");

            migrationBuilder.CreateIndex(
                name: "IX_PlatformCommissions_TenantId_StudentId_SessionId_Type",
                table: "PlatformCommissions",
                columns: new[] { "TenantId", "StudentId", "SessionId", "Type" },
                unique: true,
                filter: "[SessionId] IS NOT NULL");
        }
    }
}
