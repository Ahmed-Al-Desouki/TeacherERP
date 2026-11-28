// موجود (تعديل) — TeacherERP.Application/Interfaces/ITeamRepository.cs
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces
{
    // صف في قائمة الفريق (IsActivated = الإيميل اتأكد بالـ OTP)
    public record TeamMemberItem(
        Guid UserId,
        string FullName,
        string Email,
        MembershipStatus Status,
        bool HasFinancialDelegation,
        bool IsActivated,
        DateTime JoinedAt);

    // فريق المدرس الحالي (يتفلتر تلقائياً بالسنتر الحالي عبر الفلتر العام). الحفظ يتم عبر IUnitOfWork
    public interface ITeamRepository
    {
        // عدد مساعدي المدرس الحالي (بكل الحالات)
        Task<int> CountAssistantsAsync(CancellationToken cancellationToken);

        // هل المستخدم عضو عند المدرس الحالي (بأي دور)؟
        Task<bool> IsMemberAsync(Guid userId, CancellationToken cancellationToken);

        void AddMember(TenantMember member);

        // مساعدو المدرس الحالي فقط (بدون المدرس نفسه ولا الطلاب)، للعرض
        Task<List<TeamMemberItem>> GetAssistantsAsync(CancellationToken cancellationToken);

        // عضوية مساعد عند المدرس الحالي متتبَّعة للتعديل (null لو غير موجود أو ليس مساعداً)
        Task<TenantMember?> GetTrackedAssistantAsync(Guid userId, CancellationToken cancellationToken);

        void RemoveMember(TenantMember member);

        // هل للمستخدم عضويات عند مدرسين آخرين غير exceptTenantId؟ (عبر المدرسين بقصد)
        Task<bool> HasOtherMembershipsAsync(Guid userId, Guid exceptTenantId, CancellationToken cancellationToken);
    }
}