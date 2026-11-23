// جديد — TeacherERP.Application/Interfaces/ITeacherAccessRequestRepository.cs
using TeacherERP.Application.Common;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces
{
    // صف في قائمة طلبات المدرسين لمدير النظام (الحالة هنا هي الفعلية: المعلّق المنتهي يظهر Expired)
    public record TeacherAccessRequestListItem(
        Guid Id,
        string FullName,
        string Email,
        string Subject,
        string? Note,
        TeacherAccessRequestStatus Status,
        DateTime CreatedAt,
        DateTime ExpiresAtUtc,
        Guid? ReviewedByUserId,
        DateTime? ReviewedAtUtc);

    // طلبات انضمام المدرسين (غير معزولة بالـ Tenant). الحفظ يتم عبر IUnitOfWork
    public interface ITeacherAccessRequestRepository
    {
        // الكيانات متتبَّعة للتعديل
        Task<TeacherAccessRequest?> GetByTokenHashAsync(string tokenHash, CancellationToken cancellationToken);

        Task<TeacherAccessRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

        // كل الطلبات المعلّقة لهذا الإيميل (انتهت مدتها أم لا) لاستبدالها بطلب جديد
        Task<List<TeacherAccessRequest>> GetPendingByEmailAsync(string email, CancellationToken cancellationToken);

        // عدد الطلبات التي وافق عليها هذا المراجِع منذ تاريخ معيّن (لسقف الإدخال الدوري)
        Task<int> CountApprovedByReviewerSinceAsync(Guid reviewerUserId, DateTime sinceUtc, CancellationToken cancellationToken);

        // الأحدث أولاً، بصفحات. status فارغة = كل الحالات
        Task<PagedResult<TeacherAccessRequestListItem>> GetPageAsync(
            TeacherAccessRequestStatus? status, DateTime nowUtc, PageRequest page, CancellationToken cancellationToken);

        void Add(TeacherAccessRequest request);
    }
}
