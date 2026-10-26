// جديد — TeacherERP.Infrastructure/Repositories/TeamRepository.cs
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
    }
}