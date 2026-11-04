// جديد — TeacherERP.Application/Features/GroupAdmins/RejectGroupAdminRequest/RejectGroupAdminRequestCommand.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.GroupAdmins.RejectGroupAdminRequest
{
    // جسم الطلب (الـ Id من المسار): سبب الرفض اختياري
    public record RejectGroupAdminRequestRequest(string? Note);

    public record RejectGroupAdminRequestCommand(Guid RequestId, string? Note) : IRequest<Result<bool>>;
}