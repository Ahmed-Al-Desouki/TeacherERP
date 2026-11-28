using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;
namespace TeacherERP.Application.Features.Finance.GetMyPaymentOptions;
public sealed record GetMyPaymentOptionsQuery(Guid SessionId) : IRequest<Result<StudentPaymentDetails>>;
