// جديد — TeacherERP.Application/Features/GroupAdmins/GroupAdminDtos.cs
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Features.GroupAdmins
{
    public record GroupAdminDto(Guid StudentId, string FullName, string? ProfileImageUrl, DateTime? Since)
    {
        public static GroupAdminDto From(GroupAdminItem item) => new(item.StudentId, item.FullName, item.ProfileImageUrl, item.Since);
    }

    // الأوقات UTC. Type/Status تظهر كنص
    public record GroupAdminRequestDto(
        Guid Id,
        Guid GroupId,
        string GroupName,
        Guid StudentId,
        string StudentName,
        GroupAdminRequestType Type,
        GroupAdminRequestStatus Status,
        Guid RequestedByUserId,
        string? RequestedByName,
        DateTime CreatedAt,
        string? DecisionNote)
    {
        public static GroupAdminRequestDto From(GroupAdminRequestItem item)
            => new(item.Id, item.GroupId, item.GroupName, item.StudentId, item.StudentName, item.Type, item.Status,
                   item.RequestedByUserId, item.RequestedByName, item.CreatedAt, item.DecisionNote);
    }

    // Applied=true: اتنفّذ فوراً (200). Applied=false: طلب معلّق ينتظر المدرس (202) وRequestId هو رقمه
    public record GroupAdminChangeResultDto(bool Applied, Guid? RequestId);

    // للتطبيق: هل أنا طالب مسؤول عند المدرس الحالي، وعن أي مجموعة
    public record GroupAdminMeDto(bool IsGroupAdmin, Guid? GroupId, string? GroupName);
}
