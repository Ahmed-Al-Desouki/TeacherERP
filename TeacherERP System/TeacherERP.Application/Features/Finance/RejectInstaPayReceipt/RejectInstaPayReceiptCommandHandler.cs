using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;
namespace TeacherERP.Application.Features.Finance.RejectInstaPayReceipt;
public sealed class RejectInstaPayReceiptCommandHandler : IRequestHandler<RejectInstaPayReceiptCommand, Result<bool>>
{
    private readonly ICurrentUserService _user; private readonly ICurrentTenantAccessor _tenant; private readonly ITeacherPaymentService _payments;
    public RejectInstaPayReceiptCommandHandler(ICurrentUserService user, ICurrentTenantAccessor tenant, ITeacherPaymentService payments) { _user = user; _tenant = tenant; _payments = payments; }
    public Task<Result<bool>> Handle(RejectInstaPayReceiptCommand request, CancellationToken cancellationToken)
    {
        if (_user.UserId is not { } reviewerId) return Task.FromResult(Result<bool>.Failure(new Error("Auth.Unauthorized", "You must be logged in.")));
        if (_tenant.Current is not { } tenant) return Task.FromResult(Result<bool>.Failure(TenantErrors.Required));
        return _payments.RejectAsync(tenant.TenantId, reviewerId, request.TransactionId, cancellationToken);
    }
}
