// موجود (تعديل) — TeacherERP.Application/Features/Students/GetStudents/GetStudentsQuery.cs
using MediatR;
using TeacherERP.Application.Common;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Students.GetStudents
{
    // طلاب المدرس الحالي بصفحات (للمدرس والمساعد). تصفية اختيارية بحالة العضوية أو المجموعة
    public record GetStudentsQuery(
        MembershipStatus? Status,
        Guid? GroupId,
        int Page = 1,
        int PageSize = PageRequest.DefaultPageSize)
        : IRequest<Result<PagedResult<StudentListDto>>>, IPagedRequest;
}
