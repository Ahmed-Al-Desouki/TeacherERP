// جديد — TeacherERP.Application/Features/Attendance/GetPendingCompensations/GetPendingCompensationsQuery.cs
using MediatR;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Attendance.GetPendingCompensations
{
    // طلبات التعويض المعلّقة (المدرس الكل، والمساعد مجموعاته فقط)
    public record GetPendingCompensationsQuery : IRequest<Result<List<PendingCompensationItem>>>;
}