// موجود (تعديل) — TeacherERP.Application/Features/Enrollment/GetMyEnrollments/GetMyEnrollmentsQuery.cs
using MediatR;
using TeacherERP.Application.Common;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Enrollment.GetMyEnrollments
{
    // تسجيلاتي النشطة وطلباتي (بصفحات) عبر كل مدرسيّ. الصفحة تخص الطلبات فقط
    public record GetMyEnrollmentsQuery(int Page = 1, int PageSize = PageRequest.DefaultPageSize)
        : IRequest<Result<MyEnrollmentsDto>>, IPagedRequest;
}
