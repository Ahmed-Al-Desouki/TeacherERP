using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeacherERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixLoginSessionUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoginSessions_AspNetUsers_Id",
                table: "LoginSessions");

            migrationBuilder.DropIndex(
                name: "IX_LoginSessions_Id_DeviceId",
                table: "LoginSessions");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "LoginSessions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_LoginSessions_UserId_DeviceId",
                table: "LoginSessions",
                columns: new[] { "UserId", "DeviceId" });

            migrationBuilder.AddForeignKey(
                name: "FK_LoginSessions_AspNetUsers_UserId",
                table: "LoginSessions",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LoginSessions_AspNetUsers_UserId",
                table: "LoginSessions");

            migrationBuilder.DropIndex(
                name: "IX_LoginSessions_UserId_DeviceId",
                table: "LoginSessions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "LoginSessions");

            migrationBuilder.CreateIndex(
                name: "IX_LoginSessions_Id_DeviceId",
                table: "LoginSessions",
                columns: new[] { "Id", "DeviceId" });

            migrationBuilder.AddForeignKey(
                name: "FK_LoginSessions_AspNetUsers_Id",
                table: "LoginSessions",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
