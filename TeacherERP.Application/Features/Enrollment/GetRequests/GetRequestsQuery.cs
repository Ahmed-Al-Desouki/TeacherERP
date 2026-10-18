// جديد — TeacherERP.Application/Features/Enrollment/GetRequests/GetRequestsQuery.cs
using MediatR;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Enrollment.GetRequests
{
    // طلبات الانضمام والنقل للمدرس الحالي (المدرس الكل، والمساعد مجموعاته فقط). الحالة اختيارية
    public record GetRequestsQuery(EnrollmentRequestStatus? Status) : IRequest<Result<List<EnrollmentRequestListDto>>>;
}