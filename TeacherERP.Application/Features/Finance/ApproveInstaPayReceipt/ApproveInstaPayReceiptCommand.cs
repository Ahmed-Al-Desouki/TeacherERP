using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;
namespace TeacherERP.Application.Features.Finance.ApproveInstaPayReceipt;
public sealed record ApproveInstaPayReceiptCommand(Guid TransactionId) : IRequest<Result<CashPaymentResult>>;
