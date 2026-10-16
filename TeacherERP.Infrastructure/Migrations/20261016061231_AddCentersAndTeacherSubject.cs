using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeacherERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCentersAndTeacherSubject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CenterLatitude",
                table: "TenantSettings");

            migrationBuilder.DropColumn(
                name: "CenterLongitude",
                table: "TenantSettings");

            migrationBuilder.DropColumn(
                name: "GeofenceRadiusMeters",
                table: "TenantSettings");

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "Tenants",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Centers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    OwnerUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Latitude = table.Column<double>(type: "float", nullable: true),
                    Longitude = table.Column<double>(type: "float", nullable: true),
                    GeofenceRadiusMeters = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Centers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Centers_AspNetUsers_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CenterTeachers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CenterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    Origin = table.Column<string>(type: "nvarchar(16)", maxLength: 16, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CenterTeachers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CenterTeachers_Centers_CenterId",
                        column: x => x.CenterId,
                        principalTable: "Centers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CenterTeachers_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Centers_OwnerUserId",
                table: "Centers",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CenterTeachers_CenterId_TenantId",
                table: "CenterTeachers",
                columns: new[] { "CenterId", "TenantId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CenterTeachers_TenantId",
                table: "CenterTeachers",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CenterTeachers");

            migrationBuilder.DropTable(
                name: "Centers");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "Tenants");

            migrationBuilder.AddColumn<double>(
                name: "CenterLatitude",
                table: "TenantSettings",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "CenterLongitude",
                table: "TenantSettings",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GeofenceRadiusMeters",
                table: "TenantSettings",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
