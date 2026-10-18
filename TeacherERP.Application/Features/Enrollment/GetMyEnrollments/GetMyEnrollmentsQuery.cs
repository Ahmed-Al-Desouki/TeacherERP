// جديد — TeacherERP.Application/Features/Enrollment/GetMyEnrollments/GetMyEnrollmentsQuery.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Enrollment.GetMyEnrollments
{
    // تسجيلاتي النشطة وطلباتي الأخيرة عبر كل مدرسيّ
    public record GetMyEnrollmentsQuery : IRequest<Result<MyEnrollmentsDto>>;
}