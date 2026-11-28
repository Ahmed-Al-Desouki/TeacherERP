using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Scheduling
{
    // مهمة دورية: توليد حصص كل مدرس نشط. كل مدرس في Scope مستقل بسنتر محدد صراحةً (الـ Job ليس عنده طلب HTTP)
    public class SessionGenerationJob : ISessionGenerationJob
    {
        private readonly ITenantRepository _tenantRepository;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<SessionGenerationJob> _logger;

        public SessionGenerationJob(
            ITenantRepository tenantRepository,
            IServiceScopeFactory scopeFactory,
            ILogger<SessionGenerationJob> logger)
        {
            _tenantRepository = tenantRepository;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task RunAsync()
        {
            var tenantIds = await _tenantRepository.GetActiveTenantIdsAsync(CancellationToken.None);
            var total = 0;

            foreach (var tenantId in tenantIds)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();

                    // النظام يعمل هنا بصفة صاحب المساحة، والـ Accessor يُضبط مرة واحدة لكل Scope
                    scope.ServiceProvider.GetRequiredService<ICurrentTenantAccessor>()
                        .Set(new TenantContext(tenantId, TenantRole.Owner, MembershipStatus.Active));

                    var generator = scope.ServiceProvider.GetRequiredService<ISessionGenerator>();
                    total += await generator.GenerateForAllGroupsAsync(CancellationToken.None);
                }
                catch (Exception ex)
                {
                    // فشل مدرس واحد لا يوقف الباقي
                    _logger.LogError(ex, "Session generation failed for tenant {TenantId}.", tenantId);
                }
            }

            _logger.LogInformation("Session generation finished: {Total} new sessions for {Tenants} teachers.", total, tenantIds.Count);
        }
    }
}