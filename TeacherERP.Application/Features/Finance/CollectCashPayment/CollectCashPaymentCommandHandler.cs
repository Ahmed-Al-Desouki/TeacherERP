using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Finance.CollectCashPayment
{
    public sealed class CollectCashPaymentCommandHandler : IRequestHandler<CollectCashPaymentCommand, Result<CashPaymentResult>>
    {
        private readonly ICurrentUserService _user;
        private readonly ICurrentTenantAccessor _tenant;
        private readonly IFinancialLedgerService _ledger;
        public CollectCashPaymentCommandHandler(ICurrentUserService user, ICurrentTenantAccessor tenant, IFinancialLedgerService ledger)
        {
            _user = user;
            _tenant = tenant;
            _ledger = ledger;
        }

        public Task<Result<CashPaymentResult>> Handle(CollectCashPaymentCommand request, CancellationToken cancellationToken)
        {
            if (_user.UserId is not { } userId)
                return Task.FromResult(Result<CashPaymentResult>.Failure(new Error("Auth.Unauthorized", "You must be logged in.")));
            if (_tenant.Current is not { } tenant)
                return Task.FromResult(Result<CashPaymentResult>.Failure(TenantErrors.Required));
            return _ledger.CollectCashAsync(tenant.TenantId, userId,
                new CashPaymentRequest(request.StudentId, request.SessionId, request.PaymentType,
                    request.Amount, request.Method), cancellationToken);
        }
    }
}
