using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeacherERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWalkInLocationToEnrollmentRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "SourceLatitude",
                table: "EnrollmentRequests",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "SourceLongitude",
                table: "EnrollmentRequests",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SourceRecordedByUserId",
                table: "EnrollmentRequests",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SourceScanMethod",
                table: "EnrollmentRequests",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SourceLatitude",
                table: "EnrollmentRequests");

            migrationBuilder.DropColumn(
                name: "SourceLongitude",
                table: "EnrollmentRequests");

            migrationBuilder.DropColumn(
                name: "SourceRecordedByUserId",
                table: "EnrollmentRequests");

            migrationBuilder.DropColumn(
                name: "SourceScanMethod",
                table: "EnrollmentRequests");
        }
    }
}
