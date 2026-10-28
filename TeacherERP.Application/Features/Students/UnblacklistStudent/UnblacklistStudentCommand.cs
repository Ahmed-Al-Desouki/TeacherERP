// جديد — TeacherERP.Application/Features/Students/UnblacklistStudent/UnblacklistStudentCommand.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Students.UnblacklistStudent
{
    // فك حظر طالب (للمدرس فقط عبر الـ Policy). التسجيل لا يرجع تلقائياً، الطالب يطلب الانضمام من جديد
    public record UnblacklistStudentCommand(Guid StudentId, string Reason) : IRequest<Result<bool>>;
}