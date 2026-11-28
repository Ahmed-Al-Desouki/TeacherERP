using MediatR;
using TeacherERP.Domain.Shared;
namespace TeacherERP.Application.Features.Finance.RejectInstaPayReceipt;
public sealed record RejectInstaPayReceiptCommand(Guid TransactionId) : IRequest<Result<bool>>;
