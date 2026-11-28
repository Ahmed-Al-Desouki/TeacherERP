// موجود (تعديل) — TeacherERP.Application/Features/TeacherRequests/Review/ApproveTeacherRequestCommand.cs
using MediatR;
using TeacherERP.Application.Features.Administration;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.TeacherRequests.Review
{
    // المدرس الذي مسح الـ QR يؤكد قبول صاحب الطلب. السر نفسه هو الإثبات (لا يكفي معرّف الطلب)
    public record ApproveTeacherRequestCommand(string Token) : IRequest<Result<CreatedTeacherDto>>;
}
