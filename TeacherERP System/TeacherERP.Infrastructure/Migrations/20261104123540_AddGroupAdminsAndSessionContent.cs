using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeacherERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupAdminsAndSessionContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AutoApproveGroupAdminScans",
                table: "TenantSettings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AutoApproveGroupAdminChanges",
                table: "Tenants",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "GroupAdminSince",
                table: "GroupEnrollments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGroupAdmin",
                table: "GroupEnrollments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "GroupAdminRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    RequestedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DecidedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DecidedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DecisionNote = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupAdminRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupAdminRequests_AspNetUsers_StudentId",
                        column: x => x.StudentId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GroupAdminRequests_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GroupAdminRequests_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SessionAttachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Kind = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    PublicId = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SizeBytes = table.Column<long>(type: "bigint", nullable: false),
                    Caption = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    UploadedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SessionAttachments_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SessionAttachments_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupAdminRequests_GroupId",
                table: "GroupAdminRequests",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupAdminRequests_StudentId",
                table: "GroupAdminRequests",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupAdminRequests_TenantId_GroupId_StudentId",
                table: "GroupAdminRequests",
                columns: new[] { "TenantId", "GroupId", "StudentId" },
                unique: true,
                filter: "[Status] = 'Pending'");

            migrationBuilder.CreateIndex(
                name: "IX_GroupAdminRequests_TenantId_Status_CreatedAt",
                table: "GroupAdminRequests",
                columns: new[] { "TenantId", "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SessionAttachments_SessionId_CreatedAt",
                table: "SessionAttachments",
                columns: new[] { "SessionId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_SessionAttachments_TenantId_CreatedAt",
                table: "SessionAttachments",
                columns: new[] { "TenantId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupAdminRequests");

            migrationBuilder.DropTable(
                name: "SessionAttachments");

            migrationBuilder.DropColumn(
                name: "AutoApproveGroupAdminScans",
                table: "TenantSettings");

            migrationBuilder.DropColumn(
                name: "AutoApproveGroupAdminChanges",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "GroupAdminSince",
                table: "GroupEnrollments");

            migrationBuilder.DropColumn(
                name: "IsGroupAdmin",
                table: "GroupEnrollments");
        }
    }
}
