using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeacherERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTenants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TenantSettings_AspNetUsers_SuperAdminId",
                table: "TenantSettings");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceRecords_SessionId",
                table: "AttendanceRecords");

            migrationBuilder.RenameColumn(
                name: "SuperAdminId",
                table: "TenantSettings",
                newName: "TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_TenantSettings_SuperAdminId",
                table: "TenantSettings",
                newName: "IX_TenantSettings_TenantId");

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Transactions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<double>(
                name: "CenterLongitude",
                table: "TenantSettings",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<double>(
                name: "CenterLatitude",
                table: "TenantSettings",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Sessions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "Groups",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<double>(
                name: "ScannedLongitude",
                table: "AttendanceRecords",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<double>(
                name: "ScannedLatitude",
                table: "AttendanceRecords",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<Guid>(
                name: "RecordedByUserId",
                table: "AttendanceRecords",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "AttendanceRecords",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TimeZoneId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    IsListedInSearch = table.Column<bool>(type: "bit", nullable: false),
                    AutoApproveJoinRequests = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tenants_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TenantMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantMembers_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenantMembers_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TenantId",
                table: "Transactions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_TenantId",
                table: "Sessions",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_TenantId",
                table: "Groups",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_RecordedByUserId",
                table: "AttendanceRecords",
                column: "RecordedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_SessionId_StudentId",
                table: "AttendanceRecords",
                columns: new[] { "SessionId", "StudentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_TenantId",
                table: "AttendanceRecords",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantMembers_TenantId_UserId",
                table: "TenantMembers",
                columns: new[] { "TenantId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TenantMembers_UserId",
                table: "TenantMembers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_OwnerId",
                table: "Tenants",
                column: "OwnerId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecords_AspNetUsers_RecordedByUserId",
                table: "AttendanceRecords",
                column: "RecordedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecords_Tenants_TenantId",
                table: "AttendanceRecords",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Tenants_TenantId",
                table: "Groups",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Tenants_TenantId",
                table: "Sessions",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TenantSettings_Tenants_TenantId",
                table: "TenantSettings",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Tenants_TenantId",
                table: "Transactions",
                column: "TenantId",
                principalTable: "Tenants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecords_AspNetUsers_RecordedByUserId",
                table: "AttendanceRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecords_Tenants_TenantId",
                table: "AttendanceRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Tenants_TenantId",
                table: "Groups");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Tenants_TenantId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_TenantSettings_Tenants_TenantId",
                table: "TenantSettings");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Tenants_TenantId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "TenantMembers");

            migrationBuilder.DropTable(
                name: "Tenants");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_TenantId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_TenantId",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Groups_TenantId",
                table: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceRecords_RecordedByUserId",
                table: "AttendanceRecords");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceRecords_SessionId_StudentId",
                table: "AttendanceRecords");

            migrationBuilder.DropIndex(
                name: "IX_AttendanceRecords_TenantId",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "RecordedByUserId",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "AttendanceRecords");

            migrationBuilder.RenameColumn(
                name: "TenantId",
                table: "TenantSettings",
                newName: "SuperAdminId");

            migrationBuilder.RenameIndex(
                name: "IX_TenantSettings_TenantId",
                table: "TenantSettings",
                newName: "IX_TenantSettings_SuperAdminId");

            migrationBuilder.AlterColumn<double>(
                name: "CenterLongitude",
                table: "TenantSettings",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "CenterLatitude",
                table: "TenantSettings",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "ScannedLongitude",
                table: "AttendanceRecords",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "ScannedLatitude",
                table: "AttendanceRecords",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AttendanceRecords_SessionId",
                table: "AttendanceRecords",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_TenantSettings_AspNetUsers_SuperAdminId",
                table: "TenantSettings",
                column: "SuperAdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
