// جديد — TeacherERP.API/Extensions/RateLimitingExtensions.cs
using System.Globalization;
using System.Security.Claims;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace TeacherERP.API.Extensions
{
    public static class RateLimitingExtensions
    {
        // الأرقام كلها هنا: لو اتضح إنها بتظلم ناس عاديين نعدّلها من مكان واحد
        private const int LoginPerMinute = 20;
        private const int OtpSendPerTenMinutes = 10;
        private const int OtpVerifyPerTenMinutes = 20;
        private const int SessionPerMinute = 60;
        private const int ScanPerMinute = 20;
        private const int GlobalPerMinute = 300;

        public static IServiceCollection AddApiRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                // شبكة أمان لكل الـ endpoints: بالمستخدم لو مسجّل دخوله، وإلا بالـ IP
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        UserOrIpKey(context), _ => Window(GlobalPerMinute, TimeSpan.FromMinutes(1))));

                options.AddPolicy(RateLimitPolicies.AuthLogin, context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        IpKey(context), _ => Window(LoginPerMinute, TimeSpan.FromMinutes(1))));

                options.AddPolicy(RateLimitPolicies.AuthOtpSend, context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        IpKey(context), _ => Window(OtpSendPerTenMinutes, TimeSpan.FromMinutes(10))));

                options.AddPolicy(RateLimitPolicies.AuthOtpVerify, context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        IpKey(context), _ => Window(OtpVerifyPerTenMinutes, TimeSpan.FromMinutes(10))));

                options.AddPolicy(RateLimitPolicies.AuthSession, context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        IpKey(context), _ => Window(SessionPerMinute, TimeSpan.FromMinutes(1))));

                options.AddPolicy(RateLimitPolicies.AttendanceScan, context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        UserOrIpKey(context), _ => Window(ScanPerMinute, TimeSpan.FromMinutes(1))));

                options.OnRejected = async (context, cancellationToken) =>
                {
                    var http = context.HttpContext;
                    http.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    {
                        http.Response.Headers.RetryAfter =
                            ((int)Math.Ceiling(retryAfter.TotalSeconds)).ToString(CultureInfo.InvariantCulture);
                    }

                    // تسجيل بدون أي بيانات حساسة (لا إيميل ولا باسورد ولا توكن)
                    var policy = http.GetEndpoint()?.Metadata.GetMetadata<EnableRateLimitingAttribute>()?.PolicyName ?? "global";
                    var logger = http.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("RateLimiting");
                    logger.LogWarning(
                        "Rate limit hit. Policy {Policy}, Path {Path}, Ip {Ip}, UserId {UserId}",
                        policy, http.Request.Path.Value, ClientIp(http), UserId(http) ?? "anonymous");

                    // نفس شكل أخطاء الـ API
                    await http.Response.WriteAsJsonAsync(new
                    {
                        errorCode = "RateLimit.Exceeded",
                        errorMessage = "Too many requests. Please try again later."
                    }, cancellationToken);
                };
            });

            return services;
        }

        private static FixedWindowRateLimiterOptions Window(int permitLimit, TimeSpan window) => new()
        {
            PermitLimit = permitLimit,
            Window = window,
            QueueLimit = 0,
            AutoReplenishment = true
        };

        private static string ClientIp(HttpContext context)
        {
            var ip = context.Connection.RemoteIpAddress;
            if (ip is null)
            {
                return "unknown";
            }

            return (ip.IsIPv4MappedToIPv6 ? ip.MapToIPv4() : ip).ToString();
        }

        private static string IpKey(HttpContext context) => "ip:" + ClientIp(context);

        private static string? UserId(HttpContext context) => context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        private static string UserOrIpKey(HttpContext context)
        {
            var userId = UserId(context);
            return userId is null ? IpKey(context) : "user:" + userId;
        }
    }
}