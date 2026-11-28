// جديد — TeacherERP.Application/Features/Enrollment/RequestGroup/RequestGroupCommand.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Enrollment.RequestGroup
{
    // طلب الطالب مجموعة: انضمام لو ليس مسجلاً عند هذا المدرس، ونقل لو مسجل في مجموعة أخرى عنده
    public record RequestGroupCommand(Guid GroupId) : IRequest<Result<EnrollmentRequestDto>>;
}