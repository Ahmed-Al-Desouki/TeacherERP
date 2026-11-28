// موجود (تعديل) — TeacherERP.Application/Features/Audit/GetAuditLog/GetAuditLogQueryHandler.cs
using MediatR;
using TeacherERP.Application.Common;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Audit.GetAuditLog
{
    public class GetAuditLogQueryHandler : IRequestHandler<GetAuditLogQuery, Result<PagedResult<AuditLogDto>>>
    {
        private readonly ICurrentTenantAccessor _currentTenant;
        private readonly IAuditRepository _auditRepository;

        public GetAuditLogQueryHandler(ICurrentTenantAccessor currentTenant, IAuditRepository auditRepository)
        {
            _currentTenant = currentTenant;
            _auditRepository = auditRepository;
        }

        public async Task<Result<PagedResult<AuditLogDto>>> Handle(GetAuditLogQuery request, CancellationToken cancellationToken)
        {
            if (_currentTenant.Current is null)
            {
                return Result<PagedResult<AuditLogDto>>.Failure(TenantErrors.Required);
            }

            var page = await _auditRepository.GetPageAsync(request.TargetUserId, PageRequest.From(request), cancellationToken);

            return Result<PagedResult<AuditLogDto>>.Success(page.Map(AuditLogDto.From));
        }
    }
}
