using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TeacherERP.Infrastructure.Data
{
    // هذا الكلاس يعمل فقط وقت تشغيل أوامر الـ Migrations من الـ Terminal
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=db71872.public.databaseasp.net; Database=db71872; User Id=db71872; Password=4Qt#Fs!6r5B-; Encrypt=True; TrustServerCertificate=True; MultipleActiveResultSets=True;");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}