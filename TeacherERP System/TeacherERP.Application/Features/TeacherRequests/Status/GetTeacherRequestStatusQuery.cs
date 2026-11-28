// موجود (تعديل) — TeacherERP.Application/Features/TeacherRequests/Status/GetTeacherRequestStatusQuery.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.TeacherRequests.Status
{
    // صاحب الطلب يتابع حالته بالسر نفسه الذي في الـ QR (الشاشة تنتظر قبول المدرس)
    public record GetTeacherRequestStatusQuery(string Token) : IRequest<Result<TeacherRequestStatusDto>>;
}
