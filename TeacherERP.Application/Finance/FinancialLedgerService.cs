using System.Data;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Application.Finance
{
    public sealed class FinancialLedgerService : IFinancialLedgerService
    {
        private readonly IFinancialLedgerRepository _repository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public FinancialLedgerService(IFinancialLedgerRepository repository, IEnrollmentRepository enrollmentRepository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _enrollmentRepository = enrollmentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<FinancialPaymentOptions>> GetOptionsAsync(Guid tenantId, Guid studentId, Guid sessionId, CancellationToken cancellationToken)
        {
            var info = await _repository.GetSessionInfoAsync(tenantId, studentId, sessionId, cancellationToken);
            if (info is null || !await _repository.HasPresentAttendanceAsync(tenantId, studentId, sessionId, cancellationToken))
                return Result<FinancialPaymentOptions>.Failure(FinanceErrors.StudentOrSessionNotFound);
            var debts = await _repository.GetOutstandingObligationsAsync(tenantId, studentId, cancellationToken);
            var (year, month) = GetCalendarMonth(info);
            var hasSubscription = await _repository.HasMonthlySubscriptionAsync(tenantId, studentId, info.GroupId, year, month, cancellationToken);
            var items = debts.Select(x => new FinancialDebtItem(x.SessionId, x.GroupId, x.GroupName, x.SessionStartUtc,
                x.Amount, x.PaidAmount, x.OutstandingAmount)).ToList();
            var member = await _enrollmentRepository.GetMemberAsync(info.TenantId, studentId, cancellationToken);
            var recent = await _repository.GetRecentPaymentsAsync(tenantId, studentId, cancellationToken);
            return Result<FinancialPaymentOptions>.Success(new FinancialPaymentOptions(
                studentId, info.GroupId, info.SessionId, info.GroupName, info.PerSessionPrice, info.MonthlyPrice,
                items, items.Sum(x => x.OutstandingAmount), member?.CurrentBalance ?? 0m,
                Math.Max(0m, member?.CurrentBalance ?? 0m),
                recent.Select(x => new FinancialTransactionItem(x.Id, x.Amount, x.Type, x.Status, x.CreatedAt, x.RelatedSessionId)).ToList(),
                hasSubscription, year, month));
        }

        public async Task RecordPresentAttendanceAsync(Guid tenantId, Guid studentId, Guid sessionId,
            Guid? compensatesSessionId, CancellationToken cancellationToken)
        {
            // التعويض لا يفرض رسماً ثانياً، والحضور المكرر لا ينشئ استحقاقاً مكرراً.
            if (compensatesSessionId.HasValue) return;
            var info = await _repository.GetSessionInfoAsync(tenantId, studentId, sessionId, cancellationToken);
            if (info is null || info.TenantId != tenantId || info.PerSessionPrice is not > 0) return;
            if (await _repository.GetObligationAsync(tenantId, studentId, sessionId, cancellationToken) is not null) return;
            var period = GetCalendarMonth(info);
            if (await _repository.HasMonthlySubscriptionAsync(tenantId, studentId, info.GroupId,
                    period.Year, period.Month, cancellationToken)) return;

            var member = await _enrollmentRepository.GetMemberAsync(tenantId, studentId, cancellationToken);
            var price = info.PerSessionPrice.Value;
            var obligation = FinancialObligation.Create(tenantId, studentId, info.GroupId, sessionId,
                info.GroupName, info.SessionStartUtc, price);
            if (member is not null)
            {
                var credit = Math.Max(0m, member.CurrentBalance);
                var existingDebts = await _repository.GetOutstandingObligationsAsync(tenantId, studentId, cancellationToken);
                foreach (var debt in existingDebts)
                {
                    if (credit <= 0) break;
                    credit -= debt.ApplyPayment(credit);
                }
                var usedOnPreviousDebt = Math.Max(0m, member.CurrentBalance) - credit;
                if (usedOnPreviousDebt > 0) member.DeductBalance(usedOnPreviousDebt);
                var appliedCredit = Math.Min(credit, price);
                if (appliedCredit > 0) obligation.ApplyPayment(appliedCredit);
                member.DeductBalance(price);
            }
            _repository.AddObligation(obligation);
        }

        public Task<Result<CashPaymentResult>> CollectCashAsync(Guid tenantId, Guid processorId,
            CashPaymentRequest request, CancellationToken cancellationToken)
        {
            return _unitOfWork.ExecuteInTransactionAsync(
                () => CollectCashCoreAsync(tenantId, processorId, request, cancellationToken),
                IsolationLevel.Serializable, cancellationToken);
        }

        private async Task<Result<CashPaymentResult>> CollectCashCoreAsync(Guid tenantId, Guid processorId,
            CashPaymentRequest request, CancellationToken cancellationToken)
        {
            if (request.Amount < 0 || (request.Amount == 0 && request.PaymentType != PaymentType.MonthlySubscription)
                || decimal.Round(request.Amount, 2) != request.Amount)
                return Result<CashPaymentResult>.Failure(FinanceErrors.InvalidAmount);

            var info = await _repository.GetSessionInfoAsync(tenantId, request.StudentId, request.SessionId, cancellationToken);
            if (info is null || info.TenantId != tenantId
                || !await _repository.HasPresentAttendanceAsync(tenantId, request.StudentId, request.SessionId, cancellationToken))
                return Result<CashPaymentResult>.Failure(FinanceErrors.StudentOrSessionNotFound);

            var member = await _enrollmentRepository.GetMemberAsync(tenantId, request.StudentId, cancellationToken);
            if (member is null) return Result<CashPaymentResult>.Failure(FinanceErrors.StudentOrSessionNotFound);

            var debts = await _repository.GetOutstandingObligationsAsync(tenantId, request.StudentId, cancellationToken);
            var currentDebt = await _repository.GetObligationAsync(tenantId, request.StudentId, request.SessionId, cancellationToken);
            var previousDebts = debts.Where(x => x.SessionId != request.SessionId).ToList();
            var transactionId = Guid.NewGuid();
            var remainingDebt = debts.Sum(x => x.OutstandingAmount);
            var paymentType = request.PaymentType;
            var cashAmount = request.Amount;

            // المحفظة تُستخدم أولاً لتغطية أقدم المديونيات قبل تحصيل رسوم جديدة.
            var availableCredit = Math.Max(0m, member.CurrentBalance);
            var initialCredit = availableCredit;
            foreach (var debt in previousDebts)
            {
                if (availableCredit <= 0) break;
                availableCredit -= debt.ApplyPayment(availableCredit);
            }
            var walletUsedForPreviousDebt = initialCredit - availableCredit;
            if (walletUsedForPreviousDebt > 0) member.DeductBalance(walletUsedForPreviousDebt);

            if (paymentType == PaymentType.DebtSettlement)
            {
                if (!debts.Any(x => x.OutstandingAmount > 0))
                    return Result<CashPaymentResult>.Failure(FinanceErrors.NoOutstandingDebt);
                ApplyCashToDebts(debts, request.Amount);
                member.AddBalance(request.Amount);
            }
            else if (paymentType == PaymentType.PerSession)
            {
                if (previousDebts.Any(x => x.OutstandingAmount > 0))
                    return Result<CashPaymentResult>.Failure(FinanceErrors.PreviousDebtMustBeSettled);
                if (currentDebt is null || currentDebt.IsSettled)
                    return Result<CashPaymentResult>.Failure(FinanceErrors.SessionChargeNotFound);
                currentDebt.ApplyPayment(request.Amount);
                member.AddBalance(request.Amount);
            }
            else if (paymentType == PaymentType.MonthlySubscription)
            {
                if (previousDebts.Any(x => x.OutstandingAmount > 0))
                    return Result<CashPaymentResult>.Failure(FinanceErrors.PreviousDebtMustBeSettled);
                if (info.MonthlyPrice is not { } monthlyPrice)
                    return Result<CashPaymentResult>.Failure(FinanceErrors.MonthlyPriceNotConfigured);
                var walletCredit = Math.Max(0m, member.CurrentBalance);
                var walletContribution = Math.Min(walletCredit, monthlyPrice);
                if (request.Amount + walletContribution < monthlyPrice)
                    return Result<CashPaymentResult>.Failure(FinanceErrors.InsufficientPayment);
                var period = GetCalendarMonth(info);
                if (await _repository.HasMonthlySubscriptionAsync(tenantId, request.StudentId, info.GroupId,
                        period.Year, period.Month, cancellationToken))
                    return Result<CashPaymentResult>.Failure(FinanceErrors.SubscriptionAlreadyExists);

                // قيمة الباقة تُسجّل في الاشتراك؛ الزيادة النقدية وحدها تصبح رصيداً للمستقبل.
                if (walletContribution > 0) member.DeductBalance(walletContribution);
                if (request.Amount + walletContribution > monthlyPrice) member.AddBalance(request.Amount + walletContribution - monthlyPrice);
                if (currentDebt is not null)
                {
                    if (currentDebt.OutstandingAmount > 0) currentDebt.ApplyPayment(currentDebt.OutstandingAmount);
                    // إلغاء استحقاق حصة اليوم لأن الاشتراك الشهري يشملها.
                    member.AddBalance(info.PerSessionPrice ?? 0m);
                }
                var payment = Transaction.CreatePayment(tenantId, request.StudentId, processorId,
                    request.Amount, PaymentType.MonthlySubscription, request.SessionId);
                transactionId = payment.Id;
                _repository.AddPayment(payment);
                _repository.AddSubscription(MonthlySubscription.Create(tenantId, request.StudentId, info.GroupId,
                    period.Year, period.Month, monthlyPrice, payment.Id));
                remainingDebt = previousDebts.Sum(x => x.OutstandingAmount);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result<CashPaymentResult>.Success(new CashPaymentResult(transactionId,
                    PaymentType.MonthlySubscription, monthlyPrice, remainingDebt, true));
            }
            else
            {
                return Result<CashPaymentResult>.Failure(FinanceErrors.UnsupportedPaymentType);
            }

            var successfulPayment = Transaction.CreatePayment(tenantId, request.StudentId, processorId,
                cashAmount, paymentType, paymentType == PaymentType.PerSession ? request.SessionId : null);
            transactionId = successfulPayment.Id;
            _repository.AddPayment(successfulPayment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var outstanding = await _repository.GetOutstandingObligationsAsync(tenantId, request.StudentId, cancellationToken);
            remainingDebt = outstanding.Sum(x => x.OutstandingAmount);
            return Result<CashPaymentResult>.Success(new CashPaymentResult(transactionId, paymentType,
                cashAmount, remainingDebt, false));
        }

        private static void ApplyCashToDebts(IEnumerable<FinancialObligation> debts, decimal amount)
        {
            var remaining = amount;
            foreach (var debt in debts)
            {
                if (remaining <= 0) break;
                remaining -= debt.ApplyPayment(remaining);
            }
        }

        private static (int Year, int Month) GetCalendarMonth(FinancialSessionInfo info)
        {
            DateTime localStart;
            try
            {
                var zone = TimeZoneInfo.FindSystemTimeZoneById(info.TenantTimeZoneId);
                localStart = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(info.SessionStartUtc, DateTimeKind.Utc), zone);
            }
            catch (TimeZoneNotFoundException)
            {
                localStart = info.SessionStartUtc;
            }
            catch (InvalidTimeZoneException)
            {
                localStart = info.SessionStartUtc;
            }
            return (localStart.Year, localStart.Month);
        }
    }

    public static class FinanceErrors
    {
        public static readonly Error StudentOrSessionNotFound = new("Finance.StudentOrSessionNotFound", "The student or session was not found.");
        public static readonly Error InvalidAmount = new("Finance.InvalidAmount", "The amount must be a positive value with at most two decimal places.");
        public static readonly Error PreviousDebtMustBeSettled = new("Finance.PreviousDebtMustBeSettled", "Settle the student's previous debt before collecting a new session fee.");
        public static readonly Error SessionChargeNotFound = new("Finance.SessionChargeNotFound", "No unpaid charge exists for this session.");
        public static readonly Error NoOutstandingDebt = new("Finance.NoOutstandingDebt", "The student has no outstanding debt to settle.");
        public static readonly Error MonthlyPriceNotConfigured = new("Finance.MonthlyPriceNotConfigured", "A monthly price is not configured for this group.");
        public static readonly Error InsufficientPayment = new("Finance.InsufficientPayment", "The payment is less than the monthly subscription price.");
        public static readonly Error SubscriptionAlreadyExists = new("Finance.SubscriptionAlreadyExists", "The student already has a subscription for this group and calendar month.");
        public static readonly Error UnsupportedPaymentType = new("Finance.UnsupportedPaymentType", "This payment type is not supported by the cash payment endpoint.");
    }
}
