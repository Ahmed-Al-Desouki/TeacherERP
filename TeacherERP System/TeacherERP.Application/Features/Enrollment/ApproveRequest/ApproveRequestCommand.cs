// جديد — TeacherERP.Application/Features/Enrollment/ApproveRequest/ApproveRequestCommand.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Enrollment.ApproveRequest
{
    // موافقة المدرس أو المساعد المسؤول عن المجموعة على طلب معلّق
    public record ApproveRequestCommand(Guid RequestId) : IRequest<Result<bool>>;
}