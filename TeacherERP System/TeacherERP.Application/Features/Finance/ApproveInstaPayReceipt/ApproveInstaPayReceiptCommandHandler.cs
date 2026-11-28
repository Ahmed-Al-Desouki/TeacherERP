using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;
namespace TeacherERP.Application.Features.Finance.ApproveInstaPayReceipt;
public sealed class ApproveInstaPayReceiptCommandHandler : IRequestHandler<ApproveInstaPayReceiptCommand, Result<CashPaymentResult>>
{
    private readonly ICurrentUserService _user; private readonly ICurrentTenantAccessor _tenant; private readonly ITeacherPaymentService _payments;
    public ApproveInstaPayReceiptCommandHandler(ICurrentUserService user, ICurrentTenantAccessor tenant, ITeacherPaymentService payments) { _user = user; _tenant = tenant; _payments = payments; }
    public Task<Result<CashPaymentResult>> Handle(ApproveInstaPayReceiptCommand request, CancellationToken cancellationToken)
    {
        if (_user.UserId is not { } reviewerId) return Task.FromResult(Result<CashPaymentResult>.Failure(new Error("Auth.Unauthorized", "You must be logged in.")));
        if (_tenant.Current is not { } tenant) return Task.FromResult(Result<CashPaymentResult>.Failure(TenantErrors.Required));
        return _payments.ApproveAsync(tenant.TenantId, reviewerId, request.TransactionId, cancellationToken);
    }
}
