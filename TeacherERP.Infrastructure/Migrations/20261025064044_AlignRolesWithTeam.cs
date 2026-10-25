// موجود (تعديل) — TeacherERP.Infrastructure/Migrations/20261025064044_AlignRolesWithTeam.cs
using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TeacherERP.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlignRolesWithTeam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GroupAdmins");

            // دور Admin يصبح Assistant. لو الـ Seeder سبق وأنشأ Assistant (شغّلت التطبيق قبل التحديث)
            // ندمج المستخدمين في الدور الجديد ونحذف القديم، فلا يفقد أي مساعد دوره
            migrationBuilder.Sql(@"
DECLARE @oldId uniqueidentifier = (SELECT Id FROM AspNetRoles WHERE NormalizedName = 'ADMIN');
DECLARE @newId uniqueidentifier = (SELECT Id FROM AspNetRoles WHERE NormalizedName = 'ASSISTANT');

IF @oldId IS NOT NULL AND @newId IS NULL
BEGIN
    UPDATE AspNetRoles SET Name = 'Assistant', NormalizedName = 'ASSISTANT' WHERE Id = @oldId;
END
ELSE IF @oldId IS NOT NULL AND @newId IS NOT NULL
BEGIN
    INSERT INTO AspNetUserRoles (UserId, RoleId)
    SELECT ur.UserId, @newId
    FROM AspNetUserRoles ur
    WHERE ur.RoleId = @oldId
      AND NOT EXISTS (SELECT 1 FROM AspNetUserRoles x WHERE x.UserId = ur.UserId AND x.RoleId = @newId);

    DELETE FROM AspNetUserRoles WHERE RoleId = @oldId;
    DELETE FROM AspNetRoles WHERE Id = @oldId;
END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
UPDATE AspNetRoles SET Name = 'Admin', NormalizedName = 'ADMIN' WHERE NormalizedName = 'ASSISTANT';");

            migrationBuilder.CreateTable(
                name: "GroupAdmins",
                columns: table => new
                {
                    AssignedAdminsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupAdmins", x => new { x.AssignedAdminsId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_GroupAdmins_AspNetUsers_AssignedAdminsId",
                        column: x => x.AssignedAdminsId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupAdmins_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GroupAdmins_GroupId",
                table: "GroupAdmins",
                column: "GroupId");
        }
    }
}