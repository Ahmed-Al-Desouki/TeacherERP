// موجود (تعديل) — TeacherERP.Application/Features/TeacherRequests/Admin/ApproveTeacherRequestByAdminCommand.cs
using MediatR;
using TeacherERP.Application.Features.Administration;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.TeacherRequests.Admin
{
    // مدير النظام يوافق على طلب بمعرّفه (بلا سقف ولا تدقيق في مساحة أي مدرس)
    public record ApproveTeacherRequestByAdminCommand(Guid RequestId) : IRequest<Result<CreatedTeacherDto>>;
}
