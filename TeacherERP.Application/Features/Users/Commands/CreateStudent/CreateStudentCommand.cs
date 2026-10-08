using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Users.Commands.CreateStudent
{
    // تُرجع string (الإيميل) بدلاً من Guid لأن المستخدم لم يُنشأ بعد
    public record CreateStudentCommand(string FullName, string Email, string Password) : IRequest<Result<string>>;
}