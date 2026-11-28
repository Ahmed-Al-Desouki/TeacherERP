using MediatR;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;
namespace TeacherERP.Application.Features.Finance.SubmitInstaPayReceipt;
public sealed record SubmitInstaPayReceiptCommand(Guid SessionId, PaymentType Type, decimal Amount, Stream File, string FileName) : IRequest<Result<Guid>>;
