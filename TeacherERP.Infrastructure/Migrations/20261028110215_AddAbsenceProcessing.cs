using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeacherERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAbsenceProcessing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AbsencesProcessedAt",
                table: "Sessions",
                type: "datetime2",
                nullable: true);

            // الحصص المنتهية فعلاً قبل التطبيق تُعلَّم معالَجة، حتى لا يتولّد غياب بأثر رجعي على بيانات قديمة
            migrationBuilder.Sql(@"
                UPDATE Sessions
                SET AbsencesProcessedAt = SYSUTCDATETIME()
                WHERE EndTime <= SYSUTCDATETIME();");

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActorUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    TargetUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_TenantId_EndTime",
                table: "Sessions",
                columns: new[] { "TenantId", "EndTime" },
                filter: "[AbsencesProcessedAt] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_TenantId_CreatedAt",
                table: "AuditLogs",
                columns: new[] { "TenantId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_TenantId_TargetUserId",
                table: "AuditLogs",
                columns: new[] { "TenantId", "TargetUserId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_TenantId_EndTime",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "AbsencesProcessedAt",
                table: "Sessions");
        }
    }
}
