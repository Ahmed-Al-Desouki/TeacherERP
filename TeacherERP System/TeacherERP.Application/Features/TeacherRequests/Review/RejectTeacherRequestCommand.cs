// موجود (تعديل) — TeacherERP.Application/Features/TeacherRequests/Review/RejectTeacherRequestCommand.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.TeacherRequests.Review
{
    // المدرس الذي مسح الـ QR يرفض الطلب (السبب اختياري ويظهر لصاحب الطلب)
    public record RejectTeacherRequestCommand(string Token, string? Reason = null) : IRequest<Result<bool>>;
}
