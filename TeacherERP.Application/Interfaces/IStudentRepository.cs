// موجود (تعديل) — TeacherERP.Application/Interfaces/IStudentRepository.cs
using TeacherERP.Application.Common;
using TeacherERP.Domain.Entities;

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

    // طلاب المدرس الحالي للموظفين (الفلتر العام يعمل). بدون بيانات تواصل ولا مالية
    public interface IStudentRepository
    {
        // بصفحات، مرتبة بالاسم ثم الـ Id
        Task<PagedResult<StudentListItem>> GetStudentsPageAsync(
            MembershipStatus? status, Guid? groupId, PageRequest page, CancellationToken cancellationToken);
    }
}
