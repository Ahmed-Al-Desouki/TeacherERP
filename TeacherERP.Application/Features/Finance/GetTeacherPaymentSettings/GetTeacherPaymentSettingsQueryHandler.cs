using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;
namespace TeacherERP.Application.Features.Finance.GetTeacherPaymentSettings;
public sealed class GetTeacherPaymentSettingsQueryHandler : IRequestHandler<GetTeacherPaymentSettingsQuery, Result<TeacherPaymentSettingsDto?>>
{
    private readonly ICurrentTenantAccessor _tenant; private readonly ITeacherPaymentService _payments;
    public GetTeacherPaymentSettingsQueryHandler(ICurrentTenantAccessor tenant, ITeacherPaymentService payments) { _tenant = tenant; _payments = payments; }
    public async Task<Result<TeacherPaymentSettingsDto?>> Handle(GetTeacherPaymentSettingsQuery request, CancellationToken cancellationToken)
    {
        if (_tenant.Current is not { } tenant) return Result<TeacherPaymentSettingsDto?>.Failure(TenantErrors.Required);
        return Result<TeacherPaymentSettingsDto?>.Success(await _payments.GetSettingsAsync(tenant.TenantId, cancellationToken));
    }
}
