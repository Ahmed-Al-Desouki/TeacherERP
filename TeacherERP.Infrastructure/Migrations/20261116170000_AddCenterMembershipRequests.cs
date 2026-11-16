using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TeacherERP.Infrastructure.Data;

#nullable disable

namespace TeacherERP.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20261116170000_AddCenterMembershipRequests")]
    public partial class AddCenterMembershipRequests : Migration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
            => new SnapshotAccessor().Apply(modelBuilder);

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CenterJoinRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    LastNotifiedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ReviewedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DecisionNote = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CenterJoinRequests", x => x.Id);
                    table.ForeignKey("FK_CenterJoinRequests_Centers_CenterId", x => x.CenterId, "Centers", "Id", onDelete: ReferentialAction.Restrict);
                    table.ForeignKey("FK_CenterJoinRequests_Tenants_TenantId", x => x.TenantId, "Tenants", "Id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CenterInvitations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TokenHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    ExpiresAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AcceptedByTenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AcceptedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CenterInvitations", x => x.Id);
                    table.ForeignKey("FK_CenterInvitations_Centers_CenterId", x => x.CenterId, "Centers", "Id", onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex("IX_CenterJoinRequests_CenterId_TenantId", "CenterJoinRequests", new[] { "CenterId", "TenantId" }, unique: true, filter: "[Status] = 'Pending'");
            migrationBuilder.CreateIndex("IX_CenterJoinRequests_CenterId_Status_CreatedAt", "CenterJoinRequests", new[] { "CenterId", "Status", "CreatedAt" });
            migrationBuilder.CreateIndex("IX_CenterJoinRequests_TenantId_Status", "CenterJoinRequests", new[] { "TenantId", "Status" });
            migrationBuilder.CreateIndex("IX_CenterInvitations_CenterId_Status_CreatedAt", "CenterInvitations", new[] { "CenterId", "Status", "CreatedAt" });
            migrationBuilder.CreateIndex("IX_CenterInvitations_ExpiresAtUtc", "CenterInvitations", "ExpiresAtUtc");
            migrationBuilder.CreateIndex("IX_CenterInvitations_TokenHash", "CenterInvitations", "TokenHash", unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("CenterInvitations");
            migrationBuilder.DropTable("CenterJoinRequests");
        }

        private sealed class SnapshotAccessor : AppDbContextModelSnapshot
        {
            public void Apply(ModelBuilder modelBuilder) => base.BuildModel(modelBuilder);
        }
    }
}
