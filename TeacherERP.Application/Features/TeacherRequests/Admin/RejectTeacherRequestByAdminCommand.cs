// جديد — TeacherERP.Application/Features/TeacherRequests/Admin/RejectTeacherRequestByAdminCommand.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.TeacherRequests.Admin
{
    // مدير النظام يرفض طلباً بمعرّفه (السبب اختياري ويظهر لصاحب الطلب)
    public record RejectTeacherRequestByAdminCommand(Guid RequestId, string? Reason = null) : IRequest<Result<bool>>;
}
