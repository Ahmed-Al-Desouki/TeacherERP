// موجود (تعديل) — TeacherERP.Application/Features/Enrollment/GetRequests/GetRequestsQuery.cs
using MediatR;
using TeacherERP.Application.Common;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Enrollment.GetRequests
{
    // طلبات الانضمام والنقل للمدرس الحالي بصفحات (المدرس والمساعد يريان الكل). الحالة اختيارية، والأحدث أولاً
    public record GetRequestsQuery(EnrollmentRequestStatus? Status, int Page = 1, int PageSize = PageRequest.DefaultPageSize)
        : IRequest<Result<PagedResult<EnrollmentRequestListDto>>>, IPagedRequest;
}
