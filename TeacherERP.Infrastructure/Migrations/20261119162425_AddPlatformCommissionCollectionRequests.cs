using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeacherERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPlatformCommissionCollectionRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CollectionRequestedAtUtc",
                table: "PlatformCommissions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CollectionRequestedByUserId",
                table: "PlatformCommissions",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CollectionRequestedAtUtc",
                table: "PlatformCommissions");

            migrationBuilder.DropColumn(
                name: "CollectionRequestedByUserId",
                table: "PlatformCommissions");
        }
    }
}
