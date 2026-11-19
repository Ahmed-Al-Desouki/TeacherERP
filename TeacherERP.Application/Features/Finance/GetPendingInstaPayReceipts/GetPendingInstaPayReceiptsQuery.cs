using FluentValidation;
using MediatR;
using TeacherERP.Application.Common;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;
namespace TeacherERP.Application.Features.Finance.GetPendingInstaPayReceipts;
public sealed record GetPendingInstaPayReceiptsQuery(int Page = 1, int PageSize = PageRequest.DefaultPageSize) : IRequest<Result<PagedResult<PendingExternalPaymentItem>>>, IPagedRequest;
public sealed class GetPendingInstaPayReceiptsValidator : PagedRequestRules<GetPendingInstaPayReceiptsQuery> { }
