// جديد — TeacherERP.Application/Interfaces/IDirectoryRepository.cs
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces
{
    // دليل عام للطلاب: يقرأ عبر المدرسين بقصد (IgnoreQueryFilters) ويكشف بيانات عامة فقط لمدرسين ظاهرين في البحث
    public record DirectoryRow(Guid CenterId, string CenterName, string? CenterAddress, Guid TeacherId, string TeacherName, string? Subject);

    public record DirectoryCenterInfo(Guid Id, string Name, string? Address);

    public record TeacherProfileData(
        Guid TeacherId,
        string Name,
        string? Subject,
        string TimeZoneId,
        List<DirectoryCenterInfo> Centers,
        List<Group> Groups,
        Dictionary<Guid, int> ActiveCountByGroup);

    public interface IDirectoryRepository
    {
        Task<List<DirectoryRow>> SearchAsync(string term, int maxRows, CancellationToken cancellationToken);

        // null لو المدرس غير موجود أو غير موقوف أو غير ظاهر في البحث
        Task<TeacherProfileData?> GetTeacherProfileAsync(Guid teacherId, string? gradeLevel, CancellationToken cancellationToken);
    }
}