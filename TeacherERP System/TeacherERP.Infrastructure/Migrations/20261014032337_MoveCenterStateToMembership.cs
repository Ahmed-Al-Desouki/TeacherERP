using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeacherERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MoveCenterStateToMembership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. الأعمدة الجديدة في العضوية أولاً (الحالة الافتراضية للصفوف الموجودة: Active)
            migrationBuilder.AddColumn<int>(
                name: "ConsecutiveAbsences",
                table: "TenantMembers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "CurrentBalance",
                table: "TenantMembers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "HasFinancialDelegation",
                table: "TenantMembers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "TenantMembers",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: false,
                defaultValue: "Active");

            // 2. حماية: لو في مستخدم عنده رصيد أو غياب ولا عضوية له، لن نجد مكاناً ننقل إليه البيانات،
            // فنوقف الـ Migration بخطأ واضح (وتتراجع كلها) بدل ضياع بيانات بصمت
            migrationBuilder.Sql(@"
IF EXISTS (
    SELECT 1 FROM AspNetUsers u
    WHERE (u.CurrentBalance <> 0 OR u.ConsecutiveAbsences <> 0)
      AND NOT EXISTS (SELECT 1 FROM TenantMembers m WHERE m.UserId = u.Id))
    THROW 50001, 'Migration stopped: users with a balance or absences have no center membership to move them to.', 1;");

            // 3. نسخ البيانات من الحساب إلى عضويته (التفويض المالي للمساعد فقط، والمدرس صلاحيته بحكم دوره)
            migrationBuilder.Sql(@"
UPDATE m
SET m.ConsecutiveAbsences = u.ConsecutiveAbsences,
    m.CurrentBalance = u.CurrentBalance,
    m.HasFinancialDelegation = CASE WHEN m.Role = 'Assistant' THEN u.HasFinancialDelegation ELSE 0 END
FROM TenantMembers m
INNER JOIN AspNetUsers u ON u.Id = m.UserId;");

            // 4. بعد النسخ فقط نحذف الأعمدة القديمة من الحساب
            migrationBuilder.DropColumn(
                name: "ConsecutiveAbsences",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CurrentBalance",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "HasFinancialDelegation",
                table: "AspNetUsers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ConsecutiveAbsences",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "CurrentBalance",
                table: "AspNetUsers",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "HasFinancialDelegation",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            // رجوع بأفضل جهد: الرصيد مجموع العضويات، والغياب أكبر قيمة، والتفويض لو في أي عضوية
            migrationBuilder.Sql(@"
UPDATE u
SET u.ConsecutiveAbsences = ISNULL((SELECT MAX(m.ConsecutiveAbsences) FROM TenantMembers m WHERE m.UserId = u.Id), 0),
    u.CurrentBalance = ISNULL((SELECT SUM(m.CurrentBalance) FROM TenantMembers m WHERE m.UserId = u.Id), 0),
    u.HasFinancialDelegation = ISNULL((SELECT MAX(CAST(m.HasFinancialDelegation AS int)) FROM TenantMembers m WHERE m.UserId = u.Id), 0)
FROM AspNetUsers u;");

            migrationBuilder.DropColumn(
                name: "ConsecutiveAbsences",
                table: "TenantMembers");

            migrationBuilder.DropColumn(
                name: "CurrentBalance",
                table: "TenantMembers");

            migrationBuilder.DropColumn(
                name: "HasFinancialDelegation",
                table: "TenantMembers");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TenantMembers");
        }
    }
}