// موجود (تعديل) — TeacherERP.Application/Features/Groups/CreateGroup/CreateGroupCommand.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Groups.CreateGroup
{
    // إنشاء مجموعة بجدولها الأسبوعي الثابت دفعة واحدة (للمدرس فقط عبر الـ Policy)
    public record CreateGroupCommand(
        string Name,
        string GradeLevel,
        Guid CenterId,
        string? Room,
        int? Capacity,
        string? WhatsAppGroupLink,
        List<GroupSlotInput> Slots) : IRequest<Result<GroupDto>>, IGroupSchedule;
}