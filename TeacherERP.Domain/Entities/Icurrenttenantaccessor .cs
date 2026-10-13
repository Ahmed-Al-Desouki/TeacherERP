using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces
{
    // السنتر الحالي للطلب (أو الـ Job) وصفة المستخدم فيه
    public record TenantContext(Guid TenantId, TenantRole Role);

    // حاوية لكل Scope، بدون أي اعتماد على الداتابيز (لتفادي Circular Dependency مع AppDbContext).
    // من يحدد السنتر: الـ Policy في الـ API للطلبات، والـ Job صراحةً في الخلفية.
    public interface ICurrentTenantAccessor
    {
        TenantContext? Current { get; }

        // Guid.Empty لو مفيش سنتر محدد (الفلتر العام يرجّع نتيجة فاضية ساعتها)
        Guid TenantId { get; }

        // يُستدعى مرة واحدة لكل Scope، ومحاولة تغييره لسنتر مختلف خطأ برمجي
        void Set(TenantContext context);
    }
}