// جديد — TeacherERP.Application/Features/Directory/GetTeacherProfile/GetTeacherProfileQuery.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Directory.GetTeacherProfile
{
    // مدرس ظاهر في البحث: مجموعاته ومواعيده، مع تصفية اختيارية بالصف الدراسي
    public record GetTeacherProfileQuery(Guid TeacherId, string? GradeLevel) : IRequest<Result<TeacherProfileDto>>;
}