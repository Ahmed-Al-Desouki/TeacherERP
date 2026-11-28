// موجود (تعديل) — TeacherERP.Infrastructure/Repositories/TeamRepository.cs
using Microsoft.EntityFrameworkCore;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Infrastructure.Data;

namespace TeacherERP.Infrastructure.Repositories
{
    // فريق المدرس الحالي: الفلتر العام على TenantMembers يعمل (التحديد من الـ Policy)
    public class TeamRepository : ITeamRepository
    {
        private readonly AppDbContext _context;

        public TeamRepository(AppDbContext context)
        {
            _context = context;
        }

        public Task<int> CountAssistantsAsync(CancellationToken cancellationToken)
        {
            return _context.TenantMembers.CountAsync(m => m.Role == TenantRole.Assistant, cancellationToken);
        }

        public Task<bool> IsMemberAsync(Guid userId, CancellationToken cancellationToken)
        {
            return _context.TenantMembers.AnyAsync(m => m.UserId == userId, cancellationToken);
        }

        public void AddMember(TenantMember member) => _context.TenantMembers.Add(member);

        public Task<List<TeamMemberItem>> GetAssistantsAsync(CancellationToken cancellationToken)
        {
            return _context.TenantMembers
                .AsNoTracking()
                .Where(m => m.Role == TenantRole.Assistant)
                .OrderBy(m => m.User.FullName)
                .ThenBy(m => m.UserId)
                .Select(m => new TeamMemberItem(
                    m.UserId,
                    m.User.FullName,
                    m.User.Email ?? string.Empty,
                    m.Status,
                    m.HasFinancialDelegation,
                    m.User.EmailConfirmed,
                    m.CreatedAt))
                .ToListAsync(cancellationToken);
        }

        public Task<TenantMember?> GetTrackedAssistantAsync(Guid userId, CancellationToken cancellationToken)
        {
            return _context.TenantMembers.FirstOrDefaultAsync(
                m => m.UserId == userId && m.Role == TenantRole.Assistant, cancellationToken);
        }

        public void RemoveMember(TenantMember member) => _context.TenantMembers.Remove(member);

        // IgnoreQueryFilters مقصود: السؤال عن عضويات الحساب عند مدرسين آخرين (الفلتر العام يخفيهم)
        public Task<bool> HasOtherMembershipsAsync(Guid userId, Guid exceptTenantId, CancellationToken cancellationToken)
        {
            return _context.TenantMembers
                .IgnoreQueryFilters()
                .AnyAsync(m => m.UserId == userId && m.TenantId != exceptTenantId, cancellationToken);
        }
    }
}