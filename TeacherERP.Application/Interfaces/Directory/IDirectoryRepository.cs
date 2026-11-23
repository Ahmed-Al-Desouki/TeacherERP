// موجود (تعديل) — TeacherERP.Application/Interfaces/IDirectoryRepository.cs
using TeacherERP.Application.Common;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces
{
    // دليل عام للطلاب: يقرأ عبر المدرسين بقصد (IgnoreQueryFilters) ويكشف بيانات عامة فقط لمدرسين ظاهرين في البحث
    public record DirectoryRow(Guid CenterId, string CenterName, string? CenterAddress, Guid TeacherId, string TeacherName, string? Subject);

    // صفوف (سنتر، مدرس) لصفحة سناتر واحدة، مع العدد الكلي للسناتر المطابقة
    public record DirectorySearchPage(IReadOnlyList<DirectoryRow> Rows, int TotalCenters);

    public record DirectoryCenterInfo(Guid Id, string Name, string? Address);

    public record TeacherProfileData(
        Guid TeacherId,
        string Name,
        string? Subject,
        string TimeZoneId,
        List<DirectoryCenterInfo> Centers,
        PagedResult<Group> Groups,
        Dictionary<Guid, int> ActiveCountByGroup);

    public interface IDirectoryRepository
    {
        // الصفحة على مستوى السناتر (مرتبة بالاسم ثم الـ Id)، وداخل كل سنتر مدرسوه المطابقون
        Task<DirectorySearchPage> SearchAsync(string term, PageRequest page, CancellationToken cancellationToken);

        // null لو المدرس غير موجود أو موقوف أو غير ظاهر في البحث. الصفحة على مجموعاته (مرتبة بالاسم ثم الـ Id)
        Task<TeacherProfileData?> GetTeacherProfileAsync(
            Guid teacherId, string? gradeLevel, PageRequest page, CancellationToken cancellationToken);
    }
}
