using MediatR;
using TeacherERP.Application.Common;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;
namespace TeacherERP.Application.Features.Finance.GetPendingInstaPayReceipts;
public sealed class GetPendingInstaPayReceiptsQueryHandler : IRequestHandler<GetPendingInstaPayReceiptsQuery, Result<PagedResult<PendingExternalPaymentItem>>>
{
    private readonly ICurrentTenantAccessor _tenant; private readonly ITeacherPaymentService _payments;
    public GetPendingInstaPayReceiptsQueryHandler(ICurrentTenantAccessor tenant, ITeacherPaymentService payments) { _tenant = tenant; _payments = payments; }
    public async Task<Result<PagedResult<PendingExternalPaymentItem>>> Handle(GetPendingInstaPayReceiptsQuery request, CancellationToken cancellationToken)
    {
        if (_tenant.Current is not { } tenant) return Result<PagedResult<PendingExternalPaymentItem>>.Failure(TenantErrors.Required);
        return Result<PagedResult<PendingExternalPaymentItem>>.Success(
            await _payments.GetPendingPaymentsPageAsync(tenant.TenantId, new PageRequest(request.Page, request.PageSize), cancellationToken));
    }
}
