using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace TeacherERP.Infrastructure.Data
{
    // هذا الكلاس يعمل فقط وقت تشغيل أوامر الـ Migrations من الـ Terminal
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var basePath = FindConfigurationPath();
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
                ?? Environments.Development;

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true);

            if (environmentName.Equals(Environments.Development, StringComparison.OrdinalIgnoreCase))
            {
                configurationBuilder.AddUserSecrets<AppDbContextFactory>(optional: true);
            }

            var configuration = configurationBuilder
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' is missing. Configure it in User Secrets or the ConnectionStrings__DefaultConnection environment variable.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }

        private static string FindConfigurationPath()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var candidates = new[]
            {
                currentDirectory,
                Path.Combine(currentDirectory, "TeacherERP.API"),
                Path.GetFullPath(Path.Combine(currentDirectory, "..", "TeacherERP.API"))
            };

            return candidates.FirstOrDefault(path => File.Exists(Path.Combine(path, "appsettings.json")))
                ?? throw new InvalidOperationException(
                    "Could not locate TeacherERP.API appsettings files. Run the EF command from the solution directory or the API project directory.");
        }
    }
}
