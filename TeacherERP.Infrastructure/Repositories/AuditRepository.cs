// موجود (تعديل) — TeacherERP.Infrastructure/Repositories/AuditRepository.cs
using Microsoft.EntityFrameworkCore;
using TeacherERP.Application.Common;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Infrastructure.Data;
using TeacherERP.Infrastructure.Extensions;

namespace TeacherERP.Infrastructure.Repositories
{
    // سجل التدقيق للمدرس الحالي (الفلتر العام يعمل). الأسماء بـ Left Join لأن الحساب قد يكون اتمسح أو المنفّذ هو النظام
    public class AuditRepository : IAuditRepository
    {
        private readonly AppDbContext _context;

        public AuditRepository(AppDbContext context)
        {
            _context = context;
        }

        public void Add(AuditLog log) => _context.AuditLogs.Add(log);

        public Task<PagedResult<AuditLogItem>> GetPageAsync(Guid? targetUserId, PageRequest page, CancellationToken cancellationToken)
        {
            return (from log in _context.AuditLogs.AsNoTracking()
                    join actor in _context.Users.AsNoTracking() on log.ActorUserId equals (Guid?)actor.Id into actors
                    from actor in actors.DefaultIfEmpty()
                    join target in _context.Users.AsNoTracking() on log.TargetUserId equals (Guid?)target.Id into targets
                    from target in targets.DefaultIfEmpty()
                    where targetUserId == null || log.TargetUserId == targetUserId
                    orderby log.CreatedAt descending, log.Id
                    select new AuditLogItem(
                        log.Id,
                        log.Action,
                        log.ActorUserId,
                        actor == null ? null : actor.FullName,
                        log.TargetUserId,
                        target == null ? null : target.FullName,
                        log.Reason,
                        log.CreatedAt))
                .ToPagedAsync(page, cancellationToken);
        }
    }
}
