// جديد — TeacherERP.Application/Features/Attendance/ApproveCompensation/ApproveCompensationCommand.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Attendance.ApproveCompensation
{
    // موافقة المدرس أو المساعد المسؤول على طلب تعويض معلّق
    public record ApproveCompensationCommand(Guid RecordId) : IRequest<Result<bool>>;
}