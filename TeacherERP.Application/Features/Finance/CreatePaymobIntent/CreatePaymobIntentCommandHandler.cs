using MediatR;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Finance;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Finance.CreatePaymobIntent;

public sealed class CreatePaymobIntentCommandHandler
    : IRequestHandler<CreatePaymobIntentCommand, Result<PaymobPaymentIntentResult>>
{
    private readonly ICurrentUserService _user;
    private readonly ICurrentTenantAccessor _tenant;
    private readonly IPaymobPaymentService _payments;

    public CreatePaymobIntentCommandHandler(ICurrentUserService user, ICurrentTenantAccessor tenant,
        IPaymobPaymentService payments)
    {
        _user = user;
        _tenant = tenant;
        _payments = payments;
    }

    public Task<Result<PaymobPaymentIntentResult>> Handle(CreatePaymobIntentCommand request,
        CancellationToken cancellationToken)
    {
        if (_user.UserId is not { } studentId)
            return Task.FromResult(Result<PaymobPaymentIntentResult>.Failure(
                new Error("Auth.Unauthorized", "You must be logged in.")));
        if (_tenant.Current is not { } tenant)
            return Task.FromResult(Result<PaymobPaymentIntentResult>.Failure(TenantErrors.Required));
        if (tenant.Role != TenantRole.Student)
            return Task.FromResult(Result<PaymobPaymentIntentResult>.Failure(FinanceErrors.StudentOnly));

        return _payments.CreateIntentAsync(tenant.TenantId, studentId, request.IdempotencyKey,
            new CreatePaymobPaymentRequest(request.SessionId, request.Type, request.Amount, request.Integration),
            cancellationToken);
    }
}
