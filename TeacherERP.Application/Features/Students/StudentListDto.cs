// جديد — TeacherERP.Application/Features/Students/StudentListDto.cs
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Features.Students
{
    // طالب في قائمتي. Status (Active/Suspended/Blacklisted) تظهر كنص، والمجموعة هي تسجيله النشط (فارغة لو بلا تسجيل)
    public record StudentListDto(
        Guid StudentId,
        string FullName,
        MembershipStatus Status,
        int ConsecutiveAbsences,
        Guid? GroupId,
        string? GroupName,
        DateTime? EnrolledAt)
    {
        public static StudentListDto From(StudentListItem item)
            => new(item.StudentId, item.FullName, item.Status, item.ConsecutiveAbsences, item.GroupId, item.GroupName, item.EnrolledAt);
    }
}