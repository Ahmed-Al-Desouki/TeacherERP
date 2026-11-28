// جديد — TeacherERP.Application/Features/Enrollment/CancelRequest/CancelRequestCommand.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Enrollment.CancelRequest
{
    // الطالب يلغي طلبه المعلّق
    public record CancelRequestCommand(Guid RequestId) : IRequest<Result<bool>>;
}