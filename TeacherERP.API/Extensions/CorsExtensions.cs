// جديد — TeacherERP.API/Extensions/CorsExtensions.cs
namespace TeacherERP.API.Extensions
{
    public static class CorsExtensions
    {
        public const string PolicyName = "Frontend";

        // الدومينات من الإعدادات (Cors:AllowedOrigins). القائمة الفاضية = CORS مقفول (الافتراضي الآمن).
        // تطبيق Android مش محتاج CORS، ده لـ Flutter Web فقط.
        // TODO لبعدين: نضيف دومين الـ Flutter Web (وlocalhost للتطوير في appsettings.Development) لما يتحدد.
        public static IServiceCollection AddApiCors(this IServiceCollection services, IConfiguration configuration)
        {
            var origins = (configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>())
                .Where(origin => !string.IsNullOrWhiteSpace(origin))
                .Select(origin => origin.Trim().TrimEnd('/'))
                .Where(origin => origin != "*" && Uri.TryCreate(origin, UriKind.Absolute, out _)) // لا wildcard أبداً
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            services.AddCors(options => options.AddPolicy(PolicyName, policy =>
            {
                if (origins.Length == 0)
                {
                    return;
                }

                policy
                    .WithOrigins(origins)
                    .WithMethods("GET", "POST", "PUT", "DELETE")
                    .WithHeaders("Authorization", "Content-Type", "X-Time-Zone", "X-Teacher-Id")
                    .WithExposedHeaders("Retry-After");
            }));

            return services;
        }
    }
}