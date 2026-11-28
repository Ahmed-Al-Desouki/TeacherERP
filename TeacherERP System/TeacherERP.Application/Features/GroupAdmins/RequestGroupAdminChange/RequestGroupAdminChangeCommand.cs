// جديد — TeacherERP.Application/Features/GroupAdmins/RequestGroupAdminChange/RequestGroupAdminChangeCommand.cs
using MediatR;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.GroupAdmins.RequestGroupAdminChange
{
    // تعيين (Appoint) أو إزالة (Remove) طالب مسؤول. المدرس: فوري. المساعد: طلب يوافق عليه المدرس، إلا لو فعّل الموافقة التلقائية
    public record RequestGroupAdminChangeCommand(Guid GroupId, Guid StudentId, GroupAdminRequestType Type)
        : IRequest<Result<GroupAdminChangeResultDto>>;
}