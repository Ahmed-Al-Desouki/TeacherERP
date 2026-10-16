using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces
{
    // عضوية المستخدم في سنتر مع بيانات السنتر اللازمة للعرض والتحقق
    public record TenantMembershipInfo(Guid TenantId, string TenantName, TenantRole Role, MembershipStatus MemberStatus, TenantStatus TenantStatus, string? Subject);

    public interface ITenantRepository
    {
        // null لو المستخدم مش عضو أو السنتر غير موجود (نفس النتيجة عمداً، حتى لا نكشف وجود سناتر)
        Task<TenantMembershipInfo?> GetMembershipAsync(Guid userId, Guid tenantId, CancellationToken cancellationToken);

        // مساحة المدرس متتبَّعة للتعديل. الحفظ يتم عبر IUnitOfWork
        Task<Tenant?> GetByIdAsync(Guid tenantId, CancellationToken cancellationToken);

        // كل عضويات المستخدم (للعرض أو التحديد التلقائي لو عضوية واحدة)
        Task<List<TenantMembershipInfo>> GetMembershipsAsync(Guid userId, CancellationToken cancellationToken);
    }
}