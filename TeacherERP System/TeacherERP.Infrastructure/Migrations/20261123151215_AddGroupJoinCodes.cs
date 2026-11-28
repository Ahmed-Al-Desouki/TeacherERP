using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeacherERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupJoinCodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DynamicJoinEnabled",
                table: "Groups",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<string>(
                name: "DynamicJoinSecret",
                table: "Groups",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StaticJoinCode",
                table: "Groups",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "StaticJoinEnabled",
                table: "Groups",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_StaticJoinCode",
                table: "Groups",
                column: "StaticJoinCode",
                unique: true,
                filter: "[StaticJoinCode] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Groups_StaticJoinCode",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "DynamicJoinEnabled",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "DynamicJoinSecret",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "StaticJoinCode",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "StaticJoinEnabled",
                table: "Groups");
        }
    }
}
