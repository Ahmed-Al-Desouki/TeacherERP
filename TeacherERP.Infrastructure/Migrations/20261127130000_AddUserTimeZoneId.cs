using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TeacherERP.Infrastructure.Data;

#nullable disable

namespace TeacherERP.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20261127130000_AddUserTimeZoneId")]
    public partial class AddUserTimeZoneId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TimeZoneId",
                table: "AspNetUsers",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "UTC");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeZoneId",
                table: "AspNetUsers");
        }
    }
}
