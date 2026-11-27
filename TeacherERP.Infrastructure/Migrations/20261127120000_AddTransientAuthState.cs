using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using TeacherERP.Infrastructure.Data;

#nullable disable

namespace TeacherERP.Infrastructure.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20261127120000_AddTransientAuthState")]
public sealed class AddTransientAuthState : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "TransientAuthStates",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                Purpose = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                OtpHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                GrantHash = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                PayloadJson = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                FailedAttempts = table.Column<int>(type: "int", nullable: false),
                IsLocked = table.Column<bool>(type: "bit", nullable: false),
                IssueCount = table.Column<int>(type: "int", nullable: false),
                IssueWindowEndsAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                CodeExpiresAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                ResendAvailableAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                GrantExpiresAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                PayloadExpiresAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_TransientAuthStates", x => x.Id));

        migrationBuilder.CreateIndex("IX_TransientAuthStates_Purpose_Email", "TransientAuthStates", new[] { "Purpose", "Email" }, unique: true);
        migrationBuilder.CreateIndex("IX_TransientAuthStates_CodeExpiresAtUtc_IssueWindowEndsAtUtc_GrantExpiresAtUtc_PayloadExpiresAtUtc", "TransientAuthStates", new[] { "CodeExpiresAtUtc", "IssueWindowEndsAtUtc", "GrantExpiresAtUtc", "PayloadExpiresAtUtc" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
        => migrationBuilder.DropTable(name: "TransientAuthStates");

    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "10.0.13")
            .HasAnnotation("Relational:MaxIdentifierLength", 128);
        modelBuilder.Entity("TeacherERP.Domain.Entities.TransientAuthState", entity =>
        {
            entity.Property<Guid>("Id").ValueGeneratedOnAdd().HasColumnType("uniqueidentifier");
            entity.Property<DateTime>("CodeExpiresAtUtc").HasColumnType("datetime2");
            entity.Property<string>("Email").IsRequired().HasMaxLength(256).HasColumnType("nvarchar(256)");
            entity.Property<int>("FailedAttempts").HasColumnType("int");
            entity.Property<DateTime>("GrantExpiresAtUtc").HasColumnType("datetime2");
            entity.Property<string>("GrantHash").HasMaxLength(64).HasColumnType("nvarchar(64)");
            entity.Property<bool>("IsLocked").HasColumnType("bit");
            entity.Property<int>("IssueCount").HasColumnType("int");
            entity.Property<DateTime>("IssueWindowEndsAtUtc").HasColumnType("datetime2");
            entity.Property<string>("OtpHash").HasMaxLength(64).HasColumnType("nvarchar(64)");
            entity.Property<string>("PayloadJson").HasMaxLength(4000).HasColumnType("nvarchar(4000)");
            entity.Property<DateTime>("PayloadExpiresAtUtc").HasColumnType("datetime2");
            entity.Property<string>("Purpose").IsRequired().HasMaxLength(32).HasColumnType("nvarchar(32)");
            entity.Property<DateTime>("ResendAvailableAtUtc").HasColumnType("datetime2");
            entity.HasKey("Id");
            entity.HasIndex("CodeExpiresAtUtc", "IssueWindowEndsAtUtc", "GrantExpiresAtUtc", "PayloadExpiresAtUtc");
            entity.HasIndex("Purpose", "Email").IsUnique();
            entity.ToTable("TransientAuthStates");
        });
    }
}
