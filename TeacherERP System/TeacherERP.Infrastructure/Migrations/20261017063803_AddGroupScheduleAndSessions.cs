using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeacherERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupScheduleAndSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Sessions_GroupId",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_TenantId",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Groups_TenantId",
                table: "Groups");

            migrationBuilder.AlterColumn<string>(
                name: "HomeworkAnswerUrl",
                table: "Sessions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "BoardImageUrl",
                table: "Sessions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "SlotId",
                table: "Sessions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "WhatsAppGroupLink",
                table: "Groups",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Groups",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "GradeLevel",
                table: "Groups",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "Capacity",
                table: "Groups",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CenterId",
                table: "Groups",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Groups",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Room",
                table: "Groups",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "GroupScheduleSlots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Day = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupScheduleSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupScheduleSlots_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupScheduleSlots_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // نقل بيانات المجموعات الموجودة (لو وُجدت) قبل حذف أعمدة الجدول القديمة:
            // 1) كل مجموعة تنعقد في أول سنتر نشط للمدرس، وتبقى نشطة
            migrationBuilder.Sql(@"
UPDATE g
SET g.CenterId = c.CenterId,
    g.IsActive = 1
FROM Groups g
CROSS APPLY (
    SELECT TOP 1 ct.CenterId
    FROM CenterTeachers ct
    WHERE ct.TenantId = g.TenantId AND ct.Status = 'Active'
    ORDER BY ct.CreatedAt) c;");

            // حماية: مجموعة بلا سنتر (مدرسها بلا سنتر نشط) لا نستطيع ربطها، فنوقف الـ Migration بخطأ واضح
            migrationBuilder.Sql(@"
IF EXISTS (SELECT 1 FROM Groups WHERE CenterId = '00000000-0000-0000-0000-000000000000')
    THROW 50002, 'Migration stopped: some groups belong to a teacher with no active center. Run the center data fix first.', 1;");

            // 2) الأيام المفصولة بفاصلة تتحول لحصص في القالب (المدة = الفرق بين النهاية والبداية، وإلا 60 دقيقة)
            migrationBuilder.Sql(@"
INSERT INTO GroupScheduleSlots (Id, TenantId, GroupId, [Day], StartTime, DurationMinutes, CreatedAt, UpdatedAt)
SELECT NEWID(), x.TenantId, x.GroupId, x.DayName, x.StartTime, x.DurationMinutes, SYSUTCDATETIME(), NULL
FROM (
    SELECT DISTINCT
        g.TenantId AS TenantId,
        g.Id AS GroupId,
        LTRIM(RTRIM(d.value)) AS DayName,
        g.StartTime AS StartTime,
        CASE WHEN DATEDIFF(MINUTE, g.StartTime, g.EndTime) BETWEEN 15 AND 480
             THEN DATEDIFF(MINUTE, g.StartTime, g.EndTime)
             ELSE 60 END AS DurationMinutes
    FROM Groups g
    CROSS APPLY STRING_SPLIT(g.RecurringScheduleDays, ',') d
    WHERE LTRIM(RTRIM(d.value)) IN ('Sunday','Monday','Tuesday','Wednesday','Thursday','Friday','Saturday')
) x;");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "RecurringScheduleDays",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Groups");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_GroupId_StartTime",
                table: "Sessions",
                columns: new[] { "GroupId", "StartTime" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_SlotId",
                table: "Sessions",
                column: "SlotId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_TenantId_StartTime",
                table: "Sessions",
                columns: new[] { "TenantId", "StartTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Groups_CenterId",
                table: "Groups",
                column: "CenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_TenantId_Name",
                table: "Groups",
                columns: new[] { "TenantId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupScheduleSlots_GroupId_Day_StartTime",
                table: "GroupScheduleSlots",
                columns: new[] { "GroupId", "Day", "StartTime" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupScheduleSlots_TenantId",
                table: "GroupScheduleSlots",
                column: "TenantId");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Centers_CenterId",
                table: "Groups",
                column: "CenterId",
                principalTable: "Centers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_GroupScheduleSlots_SlotId",
                table: "Sessions",
                column: "SlotId",
                principalTable: "GroupScheduleSlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Centers_CenterId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_GroupScheduleSlots_SlotId",
                table: "Sessions");

            migrationBuilder.DropTable(
                name: "GroupScheduleSlots");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_GroupId_StartTime",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_SlotId",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_TenantId_StartTime",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Groups_CenterId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Groups_TenantId_Name",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "SlotId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "Capacity",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "CenterId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "Room",
                table: "Groups");

            migrationBuilder.AlterColumn<string>(
                name: "HomeworkAnswerUrl",
                table: "Sessions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BoardImageUrl",
                table: "Sessions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "WhatsAppGroupLink",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "GradeLevel",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            // رجوع للهيكل القديم فقط (جدول المجموعات القديم لا يحتفظ بقالب الحصص الجديد)
            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndTime",
                table: "Groups",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "RecurringScheduleDays",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartTime",
                table: "Groups",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_GroupId",
                table: "Sessions",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_TenantId",
                table: "Sessions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_TenantId",
                table: "Groups",
                column: "TenantId");
        }
    }
}