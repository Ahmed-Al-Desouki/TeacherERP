// جديد — TeacherERP.Application/Absence/AbsenceProcessingJob.cs
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Absence
{
    // مهمة دورية: معالجة غياب كل مدرس نشط. كل مدرس في Scope مستقل بسنتر محدد صراحةً (الـ Job ليس عنده طلب HTTP)
    public class AbsenceProcessingJob : IAbsenceProcessingJob
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AbsenceProcessingJob> _logger;

        public AbsenceProcessingJob(
            ITenantRepository tenantRepository,
            IServiceScopeFactory scopeFactory,
            ILogger<AbsenceProcessingJob> logger)
        {
            _tenantRepository = tenantRepository;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            var tenantIds = await _tenantRepository.GetActiveTenantIdsAsync(CancellationToken.None);

            foreach (var tenantId in tenantIds)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    // النظام يعمل هنا بصفة صاحب المساحة، والـ Accessor يُضبط مرة واحدة لكل Scope
                    scope.ServiceProvider.GetRequiredService<ICurrentTenantAccessor>()
                        .Set(new TenantContext(tenantId, TenantRole.Owner, MembershipStatus.Active));

                    await scope.ServiceProvider.GetRequiredService<IAbsenceProcessor>().ProcessAsync(CancellationToken.None);
                }
                catch (Exception ex)
                {
                    // فشل مدرس واحد لا يوقف الباقي
                    _logger.LogError(ex, "Absence processing failed for tenant {TenantId}.", tenantId);
                }
            }
        }
    }
}