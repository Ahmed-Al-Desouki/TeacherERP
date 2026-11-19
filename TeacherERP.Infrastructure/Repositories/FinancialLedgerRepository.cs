using Microsoft.EntityFrameworkCore;
using TeacherERP.Application.Interfaces;
using TeacherERP.Application.Common;
using TeacherERP.Infrastructure.Extensions;
using TeacherERP.Domain.Entities;
using TeacherERP.Infrastructure.Data;

namespace TeacherERP.Infrastructure.Repositories
{
    public sealed class FinancialLedgerRepository : IFinancialLedgerRepository
    {
        private readonly AppDbContext _context;
        public FinancialLedgerRepository(AppDbContext context) => _context = context;

        public Task<FinancialSessionInfo?> GetSessionInfoAsync(Guid tenantId, Guid studentId, Guid sessionId, CancellationToken cancellationToken)
        {
            return (from s in _context.Sessions.IgnoreQueryFilters().AsNoTracking()
                    join g in _context.Groups.IgnoreQueryFilters().AsNoTracking()
                        on new { s.TenantId, s.GroupId } equals new { g.TenantId, GroupId = g.Id }
                    where s.Id == sessionId && s.TenantId == tenantId && !s.IsCancelled
                        && _context.TenantMembers.IgnoreQueryFilters().Any(m => m.TenantId == tenantId
                            && m.UserId == studentId && m.Role == TenantRole.Student)
                    select new FinancialSessionInfo(s.TenantId, studentId, s.GroupId, s.Id,
                        g.Name, _context.Tenants.Where(t => t.Id == tenantId).Select(t => t.TimeZoneId).First(),
                        s.StartTime, g.PerSessionPrice, g.MonthlyPrice))
                .SingleOrDefaultAsync(cancellationToken);
        }

        public Task<bool> HasPresentAttendanceAsync(Guid tenantId, Guid studentId, Guid sessionId, CancellationToken cancellationToken) =>
            _context.AttendanceRecords.IgnoreQueryFilters().AnyAsync(x => x.TenantId == tenantId && x.StudentId == studentId && x.SessionId == sessionId
                && x.Status == AttendanceStatus.Present, cancellationToken);

        public Task<List<FinancialObligation>> GetOutstandingObligationsAsync(Guid tenantId, Guid studentId, CancellationToken cancellationToken) =>
            _context.FinancialObligations.IgnoreQueryFilters()
                .Where(x => x.TenantId == tenantId && x.StudentId == studentId && x.PaidAmount < x.Amount)
                .OrderBy(x => x.CreatedAt).ThenBy(x => x.Id)
                .ToListAsync(cancellationToken);

        public Task<FinancialObligation?> GetObligationAsync(Guid tenantId, Guid studentId, Guid sessionId, CancellationToken cancellationToken) =>
            _context.FinancialObligations.IgnoreQueryFilters().FirstOrDefaultAsync(x =>
                x.TenantId == tenantId && x.StudentId == studentId && x.SessionId == sessionId, cancellationToken);

        public Task<bool> HasMonthlySubscriptionAsync(Guid tenantId, Guid studentId, Guid groupId, int year, int month, CancellationToken cancellationToken) =>
            _context.MonthlySubscriptions.IgnoreQueryFilters().AnyAsync(x => x.TenantId == tenantId
                && x.StudentId == studentId && x.GroupId == groupId && x.Year == year && x.Month == month, cancellationToken);

        public Task<List<FinancialTransactionInfo>> GetRecentPaymentsAsync(Guid tenantId, Guid studentId, CancellationToken cancellationToken) =>
            _context.Transactions.IgnoreQueryFilters().AsNoTracking().Where(x => x.TenantId == tenantId && x.StudentId == studentId)
                .OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id).Take(20)
                .Select(x => new FinancialTransactionInfo(x.Id, x.Amount, x.Type, x.Status, x.CreatedAt, x.RelatedSessionId))
                .ToListAsync(cancellationToken);

        public void AddObligation(FinancialObligation obligation) => _context.FinancialObligations.Add(obligation);
        public void AddSubscription(MonthlySubscription subscription) => _context.MonthlySubscriptions.Add(subscription);
        public void AddPayment(Transaction transaction) => _context.Transactions.Add(transaction);

        public Task<TeacherPaymentSettings?> GetTeacherPaymentSettingsAsync(Guid tenantId, CancellationToken cancellationToken) =>
            _context.TeacherPaymentSettings.IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.TenantId == tenantId, cancellationToken);

        public void AddTeacherPaymentSettings(TeacherPaymentSettings settings) => _context.TeacherPaymentSettings.Add(settings);

        public Task<bool> HasPendingExternalPaymentAsync(Guid tenantId, Guid studentId, PaymentType type, Guid contextSessionId, CancellationToken cancellationToken) =>
            _context.Transactions.IgnoreQueryFilters().AnyAsync(x => x.TenantId == tenantId && x.StudentId == studentId
                && x.Type == type && x.PaymentContextSessionId == contextSessionId && x.Status == TransactionStatus.PendingApproval, cancellationToken);

        public Task<Transaction?> GetTransactionTrackedAsync(Guid tenantId, Guid transactionId, CancellationToken cancellationToken) =>
            _context.Transactions.IgnoreQueryFilters().FirstOrDefaultAsync(x =>
                x.Id == transactionId && x.TenantId == tenantId, cancellationToken);

        public Task<PagedResult<PendingExternalPaymentItem>> GetPendingExternalPaymentsPageAsync(
            Guid tenantId, PageRequest page, CancellationToken cancellationToken)
        {
            var query = from transaction in _context.Transactions.IgnoreQueryFilters().AsNoTracking()
                        join student in _context.Users.IgnoreQueryFilters().AsNoTracking()
                            on transaction.StudentId equals student.Id
                        where transaction.TenantId == tenantId
                            && transaction.Status == TransactionStatus.PendingApproval
                            && transaction.ReceiptImageUrl != null
                            && transaction.PaymentContextSessionId != null
                        orderby transaction.CreatedAt, transaction.Id
                        select new PendingExternalPaymentItem(transaction.Id, transaction.StudentId, student.FullName,
                            transaction.Amount, transaction.Type, transaction.CreatedAt, transaction.PaymentContextSessionId!.Value,
                            transaction.ReceiptImageUrl!);

            return query.ToPagedAsync(page, cancellationToken);
        }
    }
}
