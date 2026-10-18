// جديد — TeacherERP.Application/Features/Groups/UpdateGroupSchedule/UpdateGroupScheduleRequest.cs
namespace TeacherERP.Application.Features.Groups.UpdateGroupSchedule
{
    // جسم الطلب (الـ Id من المسار). EffectiveFrom تاريخ بتوقيت المدرس (اختياري، الافتراضي اليوم):
    // الحصص قبله لا تتأثر، وبعده تتبع القالب الجديد
    public record UpdateGroupScheduleRequest(DateOnly? EffectiveFrom, List<GroupSlotInput> Slots);
}