// جديد — TeacherERP.Application/Features/Groups/UpdateGroup/UpdateGroupRequest.cs
namespace TeacherERP.Application.Features.Groups.UpdateGroup
{
    // جسم الطلب (الـ Id من المسار): استبدال كامل لبيانات المجموعة، وIsActive=false يؤرشفها
    public record UpdateGroupRequest(
        string Name,
        string GradeLevel,
        Guid CenterId,
        string? Room,
        int? Capacity,
        string? WhatsAppGroupLink,
        bool IsActive);
}