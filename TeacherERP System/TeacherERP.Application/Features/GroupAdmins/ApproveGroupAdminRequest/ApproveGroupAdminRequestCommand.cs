// جديد — TeacherERP.Application/Features/GroupAdmins/ApproveGroupAdminRequest/ApproveGroupAdminRequestCommand.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.GroupAdmins.ApproveGroupAdminRequest
{
    // موافقة المدرس على طلب مساعد (للمدرس فقط عبر الـ Policy). القواعد تُفحص من جديد عند التنفيذ
    public record ApproveGroupAdminRequestCommand(Guid RequestId) : IRequest<Result<bool>>;
}