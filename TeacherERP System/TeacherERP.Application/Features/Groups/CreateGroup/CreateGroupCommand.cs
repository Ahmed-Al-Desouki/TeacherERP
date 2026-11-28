// موجود (تعديل) — TeacherERP.Application/Features/Groups/CreateGroup/CreateGroupCommand.cs
using MediatR;
using TeacherERP.Domain.Shared;
using TeacherERP.Domain.ValueObjects;

namespace TeacherERP.Application.Features.Groups.CreateGroup
{
    // إنشاء مجموعة بجدولها الأسبوعي الثابت دفعة واحدة (للمدرس فقط عبر الـ Policy)
    public record CreateGroupCommand(
        string Name,
        SchoolGrade Grade,
        Guid CenterId,
        string? Room,
        int? Capacity,
        string? WhatsAppGroupLink,
        List<GroupSlotInput> Slots,
        bool ConfirmRoomConflicts = false,
        decimal? PerSessionPrice = null,
        decimal? MonthlyPrice = null) : IRequest<Result<GroupDto>>, IGroupSchedule, IGroupPricing;
}
