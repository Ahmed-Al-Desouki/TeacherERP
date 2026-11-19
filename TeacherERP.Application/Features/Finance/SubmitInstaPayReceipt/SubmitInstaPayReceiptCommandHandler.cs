using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Finance;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;
namespace TeacherERP.Application.Features.Finance.SubmitInstaPayReceipt;
public sealed class SubmitInstaPayReceiptCommandHandler : IRequestHandler<SubmitInstaPayReceiptCommand, Result<Guid>>
{
    private readonly ICurrentUserService _user;
    private readonly ICurrentTenantAccessor _tenant;
    private readonly ITeacherPaymentService _payments;
    public SubmitInstaPayReceiptCommandHandler(ICurrentUserService user, ICurrentTenantAccessor tenant, ITeacherPaymentService payments)
    { _user = user; _tenant = tenant; _payments = payments; }
    public Task<Result<Guid>> Handle(SubmitInstaPayReceiptCommand request, CancellationToken cancellationToken)
    {
        if (_user.UserId is not { } studentId) return Task.FromResult(Result<Guid>.Failure(new Error("Auth.Unauthorized", "You must be logged in.")));
        if (_tenant.Current is not { } tenant) return Task.FromResult(Result<Guid>.Failure(TenantErrors.Required));
        if (tenant.Role != TenantRole.Student) return Task.FromResult(Result<Guid>.Failure(FinanceErrors.StudentOnly));
        return _payments.SubmitReceiptAsync(tenant.TenantId, studentId, request.SessionId, request.Type, request.Amount, request.File, request.FileName, cancellationToken);
    }
}
