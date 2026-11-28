// موجود (تعديل) — TeacherERP.Application/Features/TeacherRequests/Review/PreviewTeacherRequestQuery.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.TeacherRequests.Review
{
    // المدرس يمسح الـ QR فيرى بيانات صاحب الطلب قبل أن يقرر (لا يُنشأ أي شيء بالمسح وحده)
    public record PreviewTeacherRequestQuery(string Token) : IRequest<Result<TeacherRequestPreviewDto>>;
}
