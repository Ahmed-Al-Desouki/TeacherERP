// جديد — TeacherERP.Application/Features/Groups/UpdateGroup/UpdateGroupCommand.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Groups.UpdateGroup
{
    // تعديل بيانات المجموعة (للمدرس فقط عبر الـ Policy). الجدول الأسبوعي له تعديله المنفصل
    public record UpdateGroupCommand(
        Guid GroupId,
        string Name,
        string GradeLevel,
        Guid CenterId,
        string? Room,
        int? Capacity,
        string? WhatsAppGroupLink,
        bool IsActive) : IRequest<Result<GroupDto>>;
}