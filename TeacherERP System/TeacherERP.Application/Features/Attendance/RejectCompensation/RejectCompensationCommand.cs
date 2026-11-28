// جديد — TeacherERP.Application/Features/Attendance/RejectCompensation/RejectCompensationCommand.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Attendance.RejectCompensation
{
    // رفض المدرس أو المساعد المسؤول على طلب تعويض معلّق
    public record RejectCompensationCommand(Guid RecordId) : IRequest<Result<bool>>;
}