using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TeacherERP.Infrastructure.Data;

#nullable disable

namespace TeacherERP.Infrastructure.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261117120000_AddStructuredGroupGrades")]
public partial class AddStructuredGroupGrades : Migration
{
    protected override void BuildTargetModel(ModelBuilder modelBuilder)
        => new SnapshotAccessor().Apply(modelBuilder);

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "EducationStage", table: "Groups", type: "nvarchar(20)", maxLength: 20, nullable: true);
        migrationBuilder.AddColumn<int>(
            name: "GradeNumber", table: "Groups", type: "int", nullable: true);
        migrationBuilder.AddCheckConstraint(
            name: "CK_Groups_SchoolGrade", table: "Groups",
            sql: "([EducationStage] IS NULL AND [GradeNumber] IS NULL) OR " +
                 "([EducationStage] IS NOT NULL AND [GradeNumber] IS NOT NULL AND ((" +
                 "[EducationStage] = 'Primary' AND [GradeNumber] BETWEEN 1 AND 6) OR " +
                 "([EducationStage] IN ('Preparatory', 'Secondary') AND [GradeNumber] BETWEEN 1 AND 3)))");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropCheckConstraint(name: "CK_Groups_SchoolGrade", table: "Groups");
        migrationBuilder.DropColumn(name: "EducationStage", table: "Groups");
        migrationBuilder.DropColumn(name: "GradeNumber", table: "Groups");
    }

    private sealed class SnapshotAccessor : AppDbContextModelSnapshot
    {
        public void Apply(ModelBuilder modelBuilder) => base.BuildModel(modelBuilder);
    }
}
