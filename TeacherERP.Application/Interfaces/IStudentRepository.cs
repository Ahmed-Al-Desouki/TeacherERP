// جديد — TeacherERP.Application/Interfaces/IStudentRepository.cs
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
        Task<List<StudentListItem>> GetStudentsAsync(MembershipStatus? status, Guid? groupId, CancellationToken cancellationToken);
    }
}