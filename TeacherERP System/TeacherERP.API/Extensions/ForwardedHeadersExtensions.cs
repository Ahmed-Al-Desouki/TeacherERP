// جديد — TeacherERP.API/Extensions/ForwardedHeadersExtensions.cs
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;

namespace TeacherERP.API.Extensions
{
    public static class ForwardedHeadersExtensions
    {
        // مقفول افتراضياً. لو اتضح إن runasp.net ورا Proxy بيخبّي الـ IP الحقيقي:
        // ForwardedHeaders:Enabled = true و ForwardedHeaders:KnownProxies = [ "IP الـ Proxy" ]
        // الهيدر بيتقبل فقط لو جاي من Proxy مكتوب صراحةً (وLoopback الافتراضي)، وأي مصدر تاني بيتتجاهل.
        public static IServiceCollection AddApiForwardedHeaders(this IServiceCollection services, IConfiguration configuration)
        {
            if (!configuration.GetValue<bool>("ForwardedHeaders:Enabled"))
            {
                return services;
            }

            var proxies = configuration.GetSection("ForwardedHeaders:KnownProxies").Get<string[]>() ?? Array.Empty<string>();

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.ForwardLimit = 1; // خطوة Proxy واحدة فقط

                foreach (var proxy in proxies)
                {
                    if (IPAddress.TryParse(proxy.Trim(), out var address))
                    {
                        options.KnownProxies.Add(address);
                    }
                }
            });

            return services;
        }
    }
}