using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using TeacherERP.Infrastructure.Data;

#nullable disable

namespace TeacherERP.Infrastructure.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261118090000_AddSessionOriginalStartTime")]
public partial class AddSessionOriginalStartTime : Migration
{
    protected override void BuildTargetModel(ModelBuilder modelBuilder)
        => new SnapshotAccessor().Apply(modelBuilder);

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateTime>(
            name: "OriginalStartTime",
            table: "Sessions",
            type: "datetime2",
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "OriginalStartTime", table: "Sessions");
    }

    private sealed class SnapshotAccessor : AppDbContextModelSnapshot
    {
        public void Apply(ModelBuilder modelBuilder) => base.BuildModel(modelBuilder);
    }
}
