// جديد — TeacherERP.Application/Features/Students/UnsuspendStudent/UnsuspendStudentCommand.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Students.UnsuspendStudent
{
    // فك إيقاف طالب (للمدرس والمساعد). السبب إجباري ويُسجَّل، والعداد يُصفَّر
    public record UnsuspendStudentCommand(Guid StudentId, string Reason) : IRequest<Result<bool>>;
}