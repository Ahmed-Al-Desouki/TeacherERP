// جديد — TeacherERP.Application/Features/Audit/GetAuditLog/GetAuditLogQueryHandler.cs
using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Audit.GetAuditLog
{
    public class GetAuditLogQueryHandler : IRequestHandler<GetAuditLogQuery, Result<List<AuditLogDto>>>
    {
        private const int MaxRows = 100;

        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IAuditRepository _auditRepository;

        public GetAuditLogQueryHandler(ICurrentTenantAccessor currentTenant, IAuditRepository auditRepository)
        {
            _currentTenant = currentTenant;
            _auditRepository = auditRepository;
        }

        public async Task<Result<List<AuditLogDto>>> Handle(GetAuditLogQuery request, CancellationToken cancellationToken)
        {
            if (_currentTenant.Current is null)
            {
                return Result<List<AuditLogDto>>.Failure(TenantErrors.Required);
            }

            var items = await _auditRepository.GetRecentAsync(request.TargetUserId, MaxRows, cancellationToken);

            return Result<List<AuditLogDto>>.Success(items.Select(AuditLogDto.From).ToList());
        }
    }
}