// موجود (تعديل) — TeacherERP.Application/Features/Directory/GetTeacherProfile/GetTeacherProfileQuery.cs
using MediatR;
using TeacherERP.Application.Common;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Directory.GetTeacherProfile
{
    // مدرس ظاهر في البحث: مجموعاته (بصفحات) ومواعيده، مع تصفية اختيارية بالصف الدراسي
    public record GetTeacherProfileQuery(Guid TeacherId, string? GradeLevel, int Page = 1, int PageSize = PageRequest.DefaultPageSize)
        : IRequest<Result<TeacherProfileDto>>, IPagedRequest;
}
