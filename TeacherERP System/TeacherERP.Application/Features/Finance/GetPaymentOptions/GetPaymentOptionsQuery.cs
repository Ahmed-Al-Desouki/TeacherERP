using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Finance.GetPaymentOptions
{
    public sealed record GetPaymentOptionsQuery(Guid StudentId, Guid SessionId) : IRequest<Result<FinancialPaymentOptions>>;
}
