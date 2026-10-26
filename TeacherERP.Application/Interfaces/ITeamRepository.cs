// جديد — TeacherERP.Application/Interfaces/ITeamRepository.cs
using TeacherERP.Domain.Entities;

namespace TeacherERP.Application.Interfaces
{
    // فريق المدرس الحالي (يتفلتر تلقائياً بالسنتر الحالي عبر الفلتر العام). الحفظ يتم عبر IUnitOfWork
    public interface ITeamRepository
    {
        // عدد مساعدي المدرس الحالي (بكل الحالات)
        Task<int> CountAssistantsAsync(CancellationToken cancellationToken);

        // هل المستخدم عضو عند المدرس الحالي (بأي دور)؟
        Task<bool> IsMemberAsync(Guid userId, CancellationToken cancellationToken);

        void AddMember(TenantMember member);
    }
}