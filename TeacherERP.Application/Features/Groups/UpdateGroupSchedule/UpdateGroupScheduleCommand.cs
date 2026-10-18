// جديد — TeacherERP.Application/Features/Groups/UpdateGroupSchedule/UpdateGroupScheduleCommand.cs
using MediatR;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Features.Groups.UpdateGroupSchedule
{
    // استبدال القالب الأسبوعي للمجموعة من تاريخ معيّن (للمدرس فقط عبر الـ Policy)
    public record UpdateGroupScheduleCommand(Guid GroupId, DateOnly? EffectiveFrom, List<GroupSlotInput> Slots)
        : IRequest<Result<GroupDto>>, IGroupSchedule;
}