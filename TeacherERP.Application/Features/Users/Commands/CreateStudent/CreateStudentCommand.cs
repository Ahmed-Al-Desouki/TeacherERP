using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Users.Commands.CreateStudent
{
    // يحمل البيانات فقط (Data Transfer Object)
    public record CreateStudentCommand(
        string FullName,
        string Email,
        string PhoneNumber,
        string Password) : IRequest<Result<Guid>>;
}