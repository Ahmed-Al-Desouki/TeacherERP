// جديد — TeacherERP.Application/Features/Groups/UpdateGroup/UpdateGroupRequest.cs
using TeacherERP.Domain.ValueObjects;

namespace TeacherERP.Application.Features.Groups.UpdateGroup
{
    // جسم الطلب (الـ Id من المسار): استبدال كامل لبيانات المجموعة، وIsActive=false يؤرشفها
    public record UpdateGroupRequest(
        string Name,
        SchoolGrade Grade,
        Guid CenterId,
        string? Room,
        int? Capacity,
        string? WhatsAppGroupLink,
        bool IsActive,
        bool ConfirmRoomConflicts = false,
        decimal? PerSessionPrice = null,
        decimal? MonthlyPrice = null);
}
