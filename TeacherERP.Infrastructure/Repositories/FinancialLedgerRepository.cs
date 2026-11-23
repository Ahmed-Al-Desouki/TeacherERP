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
                && x.StudentId == studentId && x.GroupId == groupId && x.Year == year && x.Month == month
                && _context.Transactions.IgnoreQueryFilters().Any(payment => payment.Id == x.PaymentTransactionId
                    && (payment.Status == TransactionStatus.Successful
                        || payment.Status == TransactionStatus.PartiallyRefunded
                        || payment.Status == TransactionStatus.PaymobAdjustmentReview)), cancellationToken);

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
                && x.Type == type && x.PaymentContextSessionId == contextSessionId
                && x.Status == TransactionStatus.PendingApproval, cancellationToken);

        public Task<bool> HasOpenExternalPaymentAsync(Guid tenantId, Guid studentId, PaymentType type,
            Guid contextSessionId, CancellationToken cancellationToken) =>
            _context.Transactions.IgnoreQueryFilters().AnyAsync(x => x.TenantId == tenantId && x.StudentId == studentId
                && x.Type == type && x.PaymentContextSessionId == contextSessionId
                && (x.Status == TransactionStatus.PendingApproval
                    || x.Status == TransactionStatus.PaymobPending
                    || x.Status == TransactionStatus.PaymobAdjustmentReview), cancellationToken);

        public Task<Transaction?> GetPaymobPaymentByIdempotencyAsync(Guid tenantId, Guid studentId,
            string idempotencyKey, CancellationToken cancellationToken) =>
            _context.Transactions.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.TenantId == tenantId
                && x.StudentId == studentId && x.Provider == PaymentProvider.Paymob
                && x.IdempotencyKey == idempotencyKey, cancellationToken);

        public Task<Transaction?> GetPaymobPaymentForCallbackAsync(string? specialReference,
            string? providerOrderId, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(specialReference)
                && Guid.TryParse(specialReference, out var transactionId))
                return FindByReferenceOrOrderAsync(transactionId, providerOrderId, cancellationToken);

            if (string.IsNullOrWhiteSpace(providerOrderId)) return Task.FromResult<Transaction?>(null);
            return _context.Transactions.IgnoreQueryFilters().FirstOrDefaultAsync(x =>
                x.Provider == PaymentProvider.Paymob && x.ProviderOrderId == providerOrderId, cancellationToken);
        }

        private async Task<Transaction?> FindByReferenceOrOrderAsync(Guid transactionId, string? providerOrderId,
            CancellationToken cancellationToken)
        {
            var byId = await _context.Transactions.IgnoreQueryFilters().FirstOrDefaultAsync(x =>
                x.Id == transactionId && x.Provider == PaymentProvider.Paymob, cancellationToken);
            if (byId is not null || string.IsNullOrWhiteSpace(providerOrderId)) return byId;
            return await _context.Transactions.IgnoreQueryFilters().FirstOrDefaultAsync(x =>
                x.Provider == PaymentProvider.Paymob && x.ProviderOrderId == providerOrderId, cancellationToken);
        }

        public Task<bool> HasPaymentEventAsync(PaymentProvider provider, string providerEventId,
            CancellationToken cancellationToken) =>
            _context.PaymentEvents.IgnoreQueryFilters().AnyAsync(x => x.Provider == provider
                && x.ProviderEventId == providerEventId, cancellationToken);

        public void AddPaymobAdjustmentReview(PaymobAdjustmentReview review) =>
            _context.PaymobAdjustmentReviews.Add(review);

        public Task<PaymobAdjustmentReview?> GetPaymobAdjustmentReviewTrackedAsync(Guid reviewId,
            CancellationToken cancellationToken) =>
            _context.PaymobAdjustmentReviews.IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Id == reviewId, cancellationToken);

        public Task<PagedResult<PaymobAdjustmentReviewItem>> GetPendingPaymobAdjustmentReviewsPageAsync(
            PageRequest page, CancellationToken cancellationToken)
        {
            var query = from review in _context.PaymobAdjustmentReviews.IgnoreQueryFilters().AsNoTracking()
                        join tenant in _context.Tenants.IgnoreQueryFilters().AsNoTracking()
                            on review.TenantId equals tenant.Id
                        join transaction in _context.Transactions.IgnoreQueryFilters().AsNoTracking()
                            on review.TransactionId equals transaction.Id
                        join student in _context.Users.IgnoreQueryFilters().AsNoTracking()
                            on transaction.StudentId equals student.Id
                        where review.Status == PaymobAdjustmentReviewStatus.Pending
                        orderby review.CreatedAt, review.Id
                        select new PaymobAdjustmentReviewItem(review.Id, review.TenantId, tenant.Name,
                            review.TransactionId, student.Id, student.FullName, transaction.Type,
                            transaction.Amount, transaction.PaymentContextSessionId,
                            transaction.ProviderRefundedAmount, review.ProviderEventId,
                            review.Type, review.ProviderAmount, review.Currency, review.CreatedAt);
            return query.ToPagedAsync(page, cancellationToken);
        }

        public async Task<decimal> GetPendingPaymobRefundAmountAsync(Guid transactionId,
            CancellationToken cancellationToken) =>
            await _context.PaymobAdjustmentReviews.IgnoreQueryFilters()
                .Where(x => x.TransactionId == transactionId
                    && x.Status == PaymobAdjustmentReviewStatus.Pending
                    && x.Type == PaymobAdjustmentType.Refund)
                .Select(x => (decimal?)x.ProviderAmount).SumAsync(cancellationToken) ?? 0m;

        public Task<Guid?> GetOldestPendingPaymobAdjustmentReviewIdAsync(Guid transactionId,
            CancellationToken cancellationToken) =>
            _context.PaymobAdjustmentReviews.IgnoreQueryFilters()
                .Where(x => x.TransactionId == transactionId
                    && x.Status == PaymobAdjustmentReviewStatus.Pending)
                .OrderBy(x => x.CreatedAt).ThenBy(x => x.Id)
                .Select(x => (Guid?)x.Id).FirstOrDefaultAsync(cancellationToken);

        public Task<bool> HasOtherPendingPaymobAdjustmentReviewAsync(Guid transactionId, Guid reviewId,
            CancellationToken cancellationToken) =>
            _context.PaymobAdjustmentReviews.IgnoreQueryFilters().AnyAsync(x =>
                x.TransactionId == transactionId && x.Id != reviewId
                && x.Status == PaymobAdjustmentReviewStatus.Pending, cancellationToken);

        public Task<PlatformCommission?> GetPlatformCommissionByTransactionTrackedAsync(Guid transactionId,
            CancellationToken cancellationToken) =>
            _context.PlatformCommissions.IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.SourceTransactionId == transactionId, cancellationToken);

        public void AddPaymentEvent(PaymentEvent paymentEvent) => _context.PaymentEvents.Add(paymentEvent);

        public async Task<bool> HasSessionCommissionAsync(Guid tenantId, Guid studentId, Guid sessionId,
            CancellationToken cancellationToken)
        {
            // SQLite cannot reliably translate decimal inequality comparisons. Project only
            // the commission amounts for this student/session, then ignore fully reversed rows.
            var amounts = await _context.PlatformCommissions.IgnoreQueryFilters()
                .Where(x => x.TenantId == tenantId && x.StudentId == studentId
                    && x.SessionId == sessionId && x.Type == PlatformCommissionType.PerSession)
                .Select(x => x.Amount)
                .ToListAsync(cancellationToken);

            return amounts.Any(amount => amount > 0m);
        }

        public void AddPlatformCommission(PlatformCommission commission) =>
            _context.PlatformCommissions.Add(commission);

        public Task<PagedResult<PlatformCommissionItem>> GetPlatformCommissionsPageAsync(int year, int month,
            bool unsettledOnly, PageRequest page, CancellationToken cancellationToken, Guid? tenantId = null)
        {
            var fromUtc = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            var toUtc = fromUtc.AddMonths(1);
            var commissions = _context.PlatformCommissions.IgnoreQueryFilters().AsNoTracking()
                .Where(commission => commission.CreatedAt >= fromUtc && commission.CreatedAt < toUtc
                    && (tenantId == null || commission.TenantId == tenantId.Value)
                    && (!unsettledOnly || commission.SettledAtUtc == null));

            if (unsettledOnly && !IsSqliteProvider())
                commissions = commissions.Where(commission => commission.Amount > 0m);

            var query = from commission in commissions
                        join tenant in _context.Tenants.IgnoreQueryFilters().AsNoTracking()
                            on commission.TenantId equals tenant.Id
                        join teacher in _context.Users.IgnoreQueryFilters().AsNoTracking()
                            on tenant.OwnerId equals teacher.Id
                        join student in _context.Users.IgnoreQueryFilters().AsNoTracking()
                            on commission.StudentId equals student.Id
                        orderby tenant.Name, commission.CreatedAt, commission.Id
                        select new PlatformCommissionItem(commission.Id, commission.TenantId, tenant.Name,
                            teacher.Id, teacher.FullName, student.Id, student.FullName,
                            commission.SourceTransactionId, commission.SessionId, commission.Type,
                            commission.Amount, commission.CreatedAt, commission.SettledAtUtc,
                            commission.SettlementReference, commission.CollectionRequestedAtUtc);

            if (unsettledOnly && IsSqliteProvider())
                return GetPositiveUnsettledCommissionsPageAsync(query, page, cancellationToken);

            return query.ToPagedAsync(page, cancellationToken);
        }

        private static async Task<PagedResult<PlatformCommissionItem>> GetPositiveUnsettledCommissionsPageAsync(
            IQueryable<PlatformCommissionItem> query, PageRequest page, CancellationToken cancellationToken)
        {
            // SQLite decimal comparisons are unreliable; filter after materialization in the test provider.
            var outstanding = (await query.ToListAsync(cancellationToken))
                .Where(x => x.Amount > 0m)
                .ToList();
            return new PagedResult<PlatformCommissionItem>(outstanding.Skip(page.Skip).Take(page.PageSize).ToList(),
                page.Page, page.PageSize, outstanding.Count);
        }

        public async Task<List<PlatformCommissionSummary>> GetPlatformCommissionSummariesAsync(int year, int month,
            CancellationToken cancellationToken, Guid? tenantId = null)
        {
            var fromUtc = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            var toUtc = fromUtc.AddMonths(1);
            var query = from commission in _context.PlatformCommissions.IgnoreQueryFilters().AsNoTracking()
                        join tenant in _context.Tenants.IgnoreQueryFilters().AsNoTracking()
                            on commission.TenantId equals tenant.Id
                        join teacher in _context.Users.IgnoreQueryFilters().AsNoTracking()
                            on tenant.OwnerId equals teacher.Id
                        where commission.CreatedAt >= fromUtc && commission.CreatedAt < toUtc
                            && (tenantId == null || commission.TenantId == tenantId.Value)
                        group commission by new { TenantId = tenant.Id, TenantName = tenant.Name,
                            TeacherId = teacher.Id, TeacherName = teacher.FullName } into grouped
                        orderby grouped.Key.TenantName
                        select new PlatformCommissionSummary(grouped.Key.TenantId, grouped.Key.TenantName,
                            grouped.Key.TeacherId, grouped.Key.TeacherName, grouped.Count(), grouped.Sum(x => x.Amount),
                            grouped.Count(x => x.SettledAtUtc != null),
                            grouped.Sum(x => x.SettledAtUtc != null ? x.Amount : 0m),
                            grouped.Count(x => x.SettledAtUtc == null && x.Amount > 0m),
                            grouped.Sum(x => x.SettledAtUtc == null && x.Amount > 0m ? x.Amount : 0m));

            if (IsSqliteProvider())
            {
                var rows = await (from commission in _context.PlatformCommissions.IgnoreQueryFilters().AsNoTracking()
                                  join tenant in _context.Tenants.IgnoreQueryFilters().AsNoTracking()
                                      on commission.TenantId equals tenant.Id
                                  join teacher in _context.Users.IgnoreQueryFilters().AsNoTracking()
                                      on tenant.OwnerId equals teacher.Id
                                  where commission.CreatedAt >= fromUtc && commission.CreatedAt < toUtc
                                      && (tenantId == null || commission.TenantId == tenantId.Value)
                                  select new { Commission = commission, Tenant = tenant, Teacher = teacher })
                    .ToListAsync(cancellationToken);

                return rows.GroupBy(x => new
                    {
                        TenantId = x.Tenant.Id, TenantName = x.Tenant.Name,
                        TeacherId = x.Teacher.Id, TeacherName = x.Teacher.FullName
                    })
                    .OrderBy(x => x.Key.TenantName)
                    .Select(group => new PlatformCommissionSummary(group.Key.TenantId, group.Key.TenantName,
                        group.Key.TeacherId, group.Key.TeacherName, group.Count(), group.Sum(x => x.Commission.Amount),
                        group.Count(x => x.Commission.SettledAtUtc != null),
                        group.Where(x => x.Commission.SettledAtUtc != null).Sum(x => x.Commission.Amount),
                        group.Count(x => x.Commission.SettledAtUtc == null && x.Commission.Amount > 0m),
                        group.Where(x => x.Commission.SettledAtUtc == null && x.Commission.Amount > 0m)
                            .Sum(x => x.Commission.Amount)))
                    .ToList();
            }

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<List<PlatformCommission>> GetUnsettledPlatformCommissionsAsync(Guid tenantId,
            DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken)
        {
            var query = _context.PlatformCommissions.IgnoreQueryFilters().Where(x => x.TenantId == tenantId
                && x.SettledAtUtc == null && x.CreatedAt >= fromUtc && x.CreatedAt < toUtc)
                .OrderBy(x => x.CreatedAt).ThenBy(x => x.Id);

            if (IsSqliteProvider())
                return (await query.ToListAsync(cancellationToken)).Where(x => x.Amount > 0m).ToList();

            return await query.Where(x => x.Amount > 0m).ToListAsync(cancellationToken);
        }

        private bool IsSqliteProvider() =>
            string.Equals(_context.Database.ProviderName, "Microsoft.EntityFrameworkCore.Sqlite",
                StringComparison.Ordinal);

        public Task<PlatformCommissionSettings?> GetPlatformCommissionSettingsAsync(CancellationToken cancellationToken) =>
            _context.PlatformCommissionSettings.FirstOrDefaultAsync(
                x => x.Id == PlatformCommissionSettings.GlobalSettingsId, cancellationToken);

        public void AddPlatformCommissionSettings(PlatformCommissionSettings settings) =>
            _context.PlatformCommissionSettings.Add(settings);

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

        public async Task<MonthlyCollectionReport> GetMonthlyCollectionReportAsync(int year, int month,
            PageRequest transactionPage, PageRequest debtPage, CancellationToken cancellationToken,
            Guid? tenantId = null)
        {
            var fromUtc = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            var toUtc = fromUtc.AddMonths(1);
            var transactions = _context.Transactions.IgnoreQueryFilters().AsNoTracking()
                .Where(x => tenantId == null || x.TenantId == tenantId.Value);
            var occurred = transactions.Select(x => new
            {
                Transaction = x,
                OccurredAtUtc = x.Status == TransactionStatus.Successful
                    ? (x.Provider == PaymentProvider.Paymob || x.Provider == PaymentProvider.InstaPay
                        || x.ReceiptImageUrl != null ? x.UpdatedAt ?? x.CreatedAt : x.CreatedAt)
                    : (x.Status == TransactionStatus.PendingApproval || x.Status == TransactionStatus.PaymobPending
                        ? x.CreatedAt : x.UpdatedAt ?? x.CreatedAt)
            });
            var monthTransactions = occurred.Where(x => x.OccurredAtUtc >= fromUtc && x.OccurredAtUtc < toUtc
                && (x.Transaction.Status == TransactionStatus.Successful
                    || x.Transaction.Status == TransactionStatus.PendingApproval
                    || x.Transaction.Status == TransactionStatus.PaymobPending
                    || x.Transaction.Status == TransactionStatus.PaymobAdjustmentReview
                    || x.Transaction.Status == TransactionStatus.Failed
                    || x.Transaction.Status == TransactionStatus.Voided
                    || x.Transaction.Status == TransactionStatus.PartiallyRefunded
                    || x.Transaction.Status == TransactionStatus.Refunded));

            // Keep the monthly aggregates provider-neutral. SQLite used by integration tests
            // cannot reliably translate the conditional GroupBy projection used by SQL Server.
            var methodRows = await monthTransactions.Select(x => new
            {
                x.Transaction.Type,
                x.Transaction.Status,
                x.Transaction.Amount,
                x.Transaction.ProviderRefundedAmount,
                x.Transaction.Provider,
                HasReceipt = x.Transaction.ReceiptImageUrl != null
            }).ToListAsync(cancellationToken);
            var byMethod = methodRows.GroupBy(x => new
                {
                    Method = x.Provider == PaymentProvider.Paymob ? "Paymob"
                        : x.Provider == PaymentProvider.Other ? "Other"
                        : x.Provider == PaymentProvider.InstaPay || x.HasReceipt ? "InstaPay" : "Cash",
                    x.Type
                })
                .Select(group => new MonthlyPaymentMethodSummary(group.Key.Method, group.Key.Type,
                    group.Count(x => x.Status == TransactionStatus.Successful
                        || x.Status == TransactionStatus.PartiallyRefunded),
                    group.Sum(x => x.Status == TransactionStatus.Successful
                        || x.Status == TransactionStatus.PartiallyRefunded
                            ? x.Amount - x.ProviderRefundedAmount : 0m),
                    group.Count(x => x.Status == TransactionStatus.PendingApproval
                        || x.Status == TransactionStatus.PaymobPending),
                    group.Sum(x => x.Status == TransactionStatus.PendingApproval
                        || x.Status == TransactionStatus.PaymobPending ? x.Amount : 0m),
                    group.Count(x => x.Status == TransactionStatus.PaymobAdjustmentReview),
                    group.Sum(x => x.Status == TransactionStatus.PaymobAdjustmentReview ? x.Amount : 0m),
                    group.Count(x => x.Status == TransactionStatus.Failed || x.Status == TransactionStatus.Voided),
                    group.Sum(x => x.Status == TransactionStatus.Failed || x.Status == TransactionStatus.Voided
                        ? x.Amount : 0m),
                    group.Count(x => x.ProviderRefundedAmount > 0),
                    group.Sum(x => x.ProviderRefundedAmount)))
                .OrderBy(x => x.Method).ThenBy(x => x.Type).ToList();

            var transactionQuery = from entry in monthTransactions
                                   join tenant in _context.Tenants.IgnoreQueryFilters().AsNoTracking()
                                       on entry.Transaction.TenantId equals tenant.Id
                                   join teacher in _context.Users.IgnoreQueryFilters().AsNoTracking()
                                       on tenant.OwnerId equals teacher.Id
                                   join student in _context.Users.IgnoreQueryFilters().AsNoTracking()
                                       on entry.Transaction.StudentId equals student.Id
                                   orderby entry.OccurredAtUtc descending, entry.Transaction.Id
                                   select new MonthlyCollectionItem(entry.Transaction.Id, tenant.Id, tenant.Name,
                                       teacher.Id, teacher.FullName, student.Id, student.FullName,
                                       entry.Transaction.Type,
                                       entry.Transaction.Provider == PaymentProvider.Paymob ? "Paymob"
                                           : entry.Transaction.Provider == PaymentProvider.Other ? "Other"
                                           : entry.Transaction.Provider == PaymentProvider.InstaPay
                                               || entry.Transaction.ReceiptImageUrl != null ? "InstaPay" : "Cash",
                                       entry.Transaction.Status, entry.Transaction.Amount,
                                       entry.Transaction.ProviderRefundedAmount,
                                       entry.Transaction.CreatedAt, entry.Transaction.UpdatedAt,
                                       entry.OccurredAtUtc,
                                       entry.Transaction.RelatedSessionId);
            var transactionResult = await transactionQuery.ToPagedAsync(transactionPage, cancellationToken);

            var debtBase = _context.FinancialObligations.IgnoreQueryFilters().AsNoTracking()
                .Where(x => x.SessionStartUtc >= fromUtc && x.SessionStartUtc < toUtc
                    && x.PaidAmount < x.Amount
                    && (tenantId == null || x.TenantId == tenantId.Value));
            var debtCount = await debtBase.CountAsync(cancellationToken);
            var debtAmount = await debtBase.Select(x => (decimal?)(x.Amount - x.PaidAmount))
                .SumAsync(cancellationToken) ?? 0m;
            var debtQuery = from debt in _context.FinancialObligations.IgnoreQueryFilters().AsNoTracking()
                            join tenant in _context.Tenants.IgnoreQueryFilters().AsNoTracking()
                                on debt.TenantId equals tenant.Id
                            join teacher in _context.Users.IgnoreQueryFilters().AsNoTracking()
                                on tenant.OwnerId equals teacher.Id
                            join student in _context.Users.IgnoreQueryFilters().AsNoTracking()
                                on debt.StudentId equals student.Id
                            where debt.SessionStartUtc >= fromUtc && debt.SessionStartUtc < toUtc
                                && debt.PaidAmount < debt.Amount
                                && (tenantId == null || debt.TenantId == tenantId.Value)
                            orderby tenant.Name, debt.SessionStartUtc, student.FullName
                            select new MonthlyOutstandingDebtItem(tenant.Id, tenant.Name, teacher.Id,
                                teacher.FullName, student.Id, student.FullName, debt.SessionId, debt.GroupName,
                                debt.SessionStartUtc, debt.Amount, debt.PaidAmount, debt.Amount - debt.PaidAmount);
            var debtResult = await debtQuery.ToPagedAsync(debtPage, cancellationToken);

            var totals = new MonthlyCollectionTotals(
                byMethod.Sum(x => x.SuccessfulCount), byMethod.Sum(x => x.SuccessfulAmount),
                byMethod.Sum(x => x.PendingCount), byMethod.Sum(x => x.PendingAmount),
                byMethod.Sum(x => x.ReviewCount), byMethod.Sum(x => x.ReviewAmount),
                byMethod.Sum(x => x.FailedCount), byMethod.Sum(x => x.FailedAmount),
                byMethod.Sum(x => x.RefundedCount), byMethod.Sum(x => x.RefundedAmount),
                debtCount, debtAmount);
            return new MonthlyCollectionReport(year, month, totals, byMethod, transactionResult, debtResult);
        }
    }
}
