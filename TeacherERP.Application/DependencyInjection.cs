using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TeacherERP.Application.Behaviors;
using TeacherERP.Application.Features.Profile;
using TeacherERP.Application.Interfaces;
using TeacherERP.Application.Sessions;

namespace TeacherERP.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;

            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(assembly);
                config.AddOpenBehavior(typeof(LoggingBehavior<,>));
                config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            services.AddValidatorsFromAssembly(assembly);

            // مصدر الوقت (يمكن استبداله بـ FakeTimeProvider في الاختبارات)
            services.TryAddSingleton(TimeProvider.System);

            services.AddScoped<ISessionManager, SessionManager>();
            services.AddScoped<ISessionCleanupJob, SessionCleanupJob>();
            services.AddScoped<IProfileCompletionChecker, ProfileCompletionChecker>();

            return services;
        }
    }
}