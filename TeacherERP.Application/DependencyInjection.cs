// موجود (تعديل) — TeacherERP.Application/DependencyInjection.cs
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using TeacherERP.Application.Absence;
using TeacherERP.Application.Attendance;
using TeacherERP.Application.Behaviors;
using TeacherERP.Application.Enrollment;
using TeacherERP.Application.Features.Profile;
using TeacherERP.Application.Files;
using TeacherERP.Application.GroupAdmins;
using TeacherERP.Application.Interfaces;
using TeacherERP.Application.Scheduling;
using TeacherERP.Application.Sessions;
using TeacherERP.Application.Teachers;
using TeacherERP.Application.Centers;
using TeacherERP.Application.Finance;
using TeacherERP.Application.Services.Users;

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
            services.AddScoped<ITransientAuthStateCleanupJob, TransientAuthStateCleanupJob>();
            services.AddScoped<IProfileCompletionChecker, ProfileCompletionChecker>();
            services.AddScoped<ISessionGenerator, SessionGenerator>();
            services.AddScoped<ISessionGenerationJob, SessionGenerationJob>();
            services.AddScoped<IEnrollmentService, EnrollmentService>();
            services.AddScoped<IGroupJoinCodeService, GroupJoinCodeService>();
            services.AddScoped<IAttendanceCodeService, AttendanceCodeService>();
            services.AddScoped<IAttendanceRecorder, AttendanceRecorder>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IAbsenceService, AbsenceService>();
            services.AddScoped<IAbsenceProcessor, AbsenceProcessor>();
            services.AddScoped<IAbsenceProcessingJob, AbsenceProcessingJob>();
            services.AddScoped<IGroupAccessService, GroupAccessService>();
            services.AddScoped<IGroupAdminService, GroupAdminService>();
            services.AddScoped<IStoredFileCleanupJob, StoredFileCleanupJob>();
            services.AddScoped<ITeacherProvisioningService, TeacherProvisioningService>();
            services.AddScoped<ITeacherRequestReviewService, TeacherRequestReviewService>();
            services.AddScoped<ICenterProvisioningService, CenterProvisioningService>();
            services.AddScoped<ICenterMembershipCleanupJob, CenterMembershipCleanupJob>();
            services.AddScoped<ICenterJoinRequestNotifier, CenterJoinRequestNotifier>();
            services.AddScoped<IFinancialLedgerService, FinancialLedgerService>();
            services.AddScoped<ITeacherPaymentService, TeacherPaymentService>();
            services.AddScoped<IPaymobPaymentService, PaymobPaymentService>();
            services.AddScoped<IPaymobAdjustmentReviewService, PaymobAdjustmentReviewService>();
            services.AddScoped<IPlatformCommissionService, PlatformCommissionService>();
            services.AddScoped<IPlatformCommissionSettingsService, PlatformCommissionSettingsService>();
            services.AddScoped<IMonthlyCollectionReportService, MonthlyCollectionReportService>();

            return services;
        }
    }
}
