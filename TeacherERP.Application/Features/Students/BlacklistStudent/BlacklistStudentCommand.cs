// جديد — TeacherERP.Application/Features/Students/BlacklistStudent/BlacklistStudentCommand.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Students.BlacklistStudent
{
    // حظر طالب من هذا المدرس وإنهاء تسجيله الحالي (للمدرس فقط عبر الـ Policy)
    public record BlacklistStudentCommand(Guid StudentId, string Reason) : IRequest<Result<bool>>;
}