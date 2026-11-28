using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Finance.GetPaymentOptions
{
    public sealed class GetPaymentOptionsQueryHandler : IRequestHandler<GetPaymentOptionsQuery, Result<FinancialPaymentOptions>>
    {
        private readonly ICurrentTenantAccessor _tenant;
        private readonly IFinancialLedgerService _ledger;
        public GetPaymentOptionsQueryHandler(ICurrentTenantAccessor tenant, IFinancialLedgerService ledger)
        {
            _tenant = tenant;
            _ledger = ledger;
        }

        public Task<Result<FinancialPaymentOptions>> Handle(GetPaymentOptionsQuery request, CancellationToken cancellationToken)
        {
            if (_tenant.Current is not { } tenant)
                return Task.FromResult(Result<FinancialPaymentOptions>.Failure(TenantErrors.Required));
            return _ledger.GetOptionsAsync(tenant.TenantId, request.StudentId, request.SessionId, cancellationToken);
        }
    }
}
