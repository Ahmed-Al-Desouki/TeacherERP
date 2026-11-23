// موجود (تعديل) — TeacherERP.Application/Interfaces/IStudentRepository.cs
using TeacherERP.Application.Common;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.ValueObjects;

namespace TeacherERP.Application.Interfaces
{
    public record StudentListItem(
        Guid StudentId,
        string FullName,
        MembershipStatus Status,
        int ConsecutiveAbsences,
        Guid? GroupId,
        string? GroupName,
        DateTime? EnrolledAt);

    public record StudentActiveGroupItem(Guid GroupId, string GroupName, DateTime EnrolledAt);

    public record StudentDetailsItem(
        Guid StudentId,
        string FullName,
        string? Email,
        string? PhoneNumber,
        Gender? Gender,
        DateOnly? DateOfBirth,
        MembershipStatus MembershipStatus,
        int ConsecutiveAbsences,
        string? GuardianName,
        string? GuardianPhoneNumber,
        GuardianRelation? GuardianRelation,
        int ProfileCompletionPercentage,
        IReadOnlyList<StudentActiveGroupItem> ActiveGroups);

    // طلاب المدرس الحالي للموظفين (الفلتر العام يعمل). بدون بيانات تواصل ولا مالية
    public interface IStudentRepository
    {
        // بصفحات، مرتبة بالاسم ثم الـ Id
        Task<PagedResult<StudentListItem>> GetStudentsPageAsync(
            MembershipStatus? status, Guid? groupId, PageRequest page, CancellationToken cancellationToken);

        Task<StudentDetailsItem?> GetStudentDetailsAsync(Guid studentId, CancellationToken cancellationToken);
    }
}
