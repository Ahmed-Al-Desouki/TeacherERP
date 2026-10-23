// موجود (تعديل) — TeacherERP.Infrastructure/Migrations/<timestamp>_AddNotificationANDAttendanceSystem.cs (استبدل المحتوى فقط، واترك اسم الملف)
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeacherERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationANDAttendanceSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AutoRegisterOnScan",
                table: "TenantSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "QrSecret",
                table: "TenantSettings",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            // الصفوف الموجودة: سر عشوائي قوي لكل مدرس (32 بايت Hex)، والتسجيل التلقائي بالمسح مفعّل كالمدرسين الجدد
            migrationBuilder.Sql(@"
UPDATE TenantSettings
SET QrSecret = LOWER(CONVERT(varchar(64), CRYPT_GEN_RANDOM(32), 2)),
    AutoRegisterOnScan = 1;");

            // حماية: لا سر فارغ ولا مكرر بين مدرسين (السر هو هوية الكود)
            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM TenantSettings GROUP BY QrSecret HAVING COUNT(*) > 1 OR QrSecret = '')
    THROW 50003, 'Migration stopped: attendance secrets were not generated uniquely.', 1;");

            migrationBuilder.AddColumn<Guid>(
                name: "CompensatesSessionId",
                table: "AttendanceRecords",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecipientUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    RelatedId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_AspNetUsers_RecipientUserId",
                        column: x => x.RecipientUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notifications_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_CompensatesSessionId",
                table: "AttendanceRecords",
                column: "CompensatesSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RecipientUserId_IsRead_CreatedAt",
                table: "Notifications",
                columns: new[] { "RecipientUserId", "IsRead", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TenantId",
                table: "Notifications",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecords_Sessions_CompensatesSessionId",
                table: "AttendanceRecords",
                column: "CompensatesSessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecords_Sessions_CompensatesSessionId",
                table: "AttendanceRecords");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceRecords_CompensatesSessionId",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "AutoRegisterOnScan",
                table: "TenantSettings");

            migrationBuilder.DropColumn(
                name: "QrSecret",
                table: "TenantSettings");

            migrationBuilder.DropColumn(
                name: "CompensatesSessionId",
                table: "AttendanceRecords");
        }
    }
}