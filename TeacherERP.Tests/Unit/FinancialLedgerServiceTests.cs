using System.Data;
using NSubstitute;
using TeacherERP.Application.Finance;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;
using Xunit;

namespace TeacherERP.Tests.Unit
{
    public class FinancialLedgerServiceTests
    {
        private static readonly Guid TenantId = Guid.NewGuid();
        private static readonly Guid StudentId = Guid.NewGuid();
        private static readonly Guid SessionId = Guid.NewGuid();
        private static readonly Guid GroupId = Guid.NewGuid();

        [Fact]
        public void FinancialObligation_AppliesPartialPaymentsWithoutLosingRemainingDebt()
        {
            var obligation = FinancialObligation.Create(TenantId, StudentId, GroupId, SessionId, "Math group", new DateTime(2026, 11, 18), 75m);

            var applied = obligation.ApplyPayment(30m);

            Assert.Equal(30m, applied);
            Assert.Equal(45m, obligation.OutstandingAmount);
            Assert.False(obligation.IsSettled);
        }

        [Fact]
        public async Task RecordPresentAttendance_CreatesOneSessionDebtAndDeductsWalletCredit()
        {
            var (service, repository, enrollment, _) = CreateService();
            var member = TenantMember.Create(TenantId, StudentId, TenantRole.Student);
            member.AddBalance(20m);
            enrollment.GetMemberAsync(TenantId, StudentId, Arg.Any<CancellationToken>()).Returns(member);
            repository.GetSessionInfoAsync(TenantId, StudentId, SessionId, Arg.Any<CancellationToken>())
                .Returns(SessionInfo(perSession: 50m));
            repository.GetOutstandingObligationsAsync(TenantId, StudentId, Arg.Any<CancellationToken>()).Returns(new List<FinancialObligation>());

            await service.RecordPresentAttendanceAsync(TenantId, StudentId, SessionId, null, CancellationToken.None);

            repository.Received(1).AddObligation(Arg.Is<FinancialObligation>(x =>
                x.Amount == 50m && x.PaidAmount == 20m && x.OutstandingAmount == 30m));
            Assert.Equal(-30m, member.CurrentBalance);
        }

        [Fact]
        public async Task RecordPresentAttendance_DoesNotChargeMonthlyCoveredOrCompensationSessions()
        {
            var (service, repository, _, _) = CreateService();
            repository.GetSessionInfoAsync(TenantId, StudentId, SessionId, Arg.Any<CancellationToken>())
                .Returns(SessionInfo(perSession: 50m));
            repository.HasMonthlySubscriptionAsync(TenantId, StudentId, GroupId, 2026, 11, Arg.Any<CancellationToken>()).Returns(true);

            await service.RecordPresentAttendanceAsync(TenantId, StudentId, SessionId, null, CancellationToken.None);
            await service.RecordPresentAttendanceAsync(TenantId, StudentId, SessionId, Guid.NewGuid(), CancellationToken.None);

            repository.DidNotReceive().AddObligation(Arg.Any<FinancialObligation>());
        }

        [Fact]
        public async Task RecordPresentAttendance_UsesTeachersLocalCalendarMonth()
        {
            var (service, repository, _, _) = CreateService();
            repository.GetSessionInfoAsync(TenantId, StudentId, SessionId, Arg.Any<CancellationToken>())
                .Returns(new FinancialSessionInfo(TenantId, StudentId, GroupId, SessionId, "Math group", "Africa/Cairo",
                    new DateTime(2026, 10, 31, 22, 30, 0, DateTimeKind.Utc), 50m, 500m));
            repository.HasMonthlySubscriptionAsync(TenantId, StudentId, GroupId, 2026, 11, Arg.Any<CancellationToken>()).Returns(true);

            await service.RecordPresentAttendanceAsync(TenantId, StudentId, SessionId, null, CancellationToken.None);

            await repository.Received(1).HasMonthlySubscriptionAsync(TenantId, StudentId, GroupId, 2026, 11, Arg.Any<CancellationToken>());
            repository.DidNotReceive().AddObligation(Arg.Any<FinancialObligation>());
        }

        [Fact]
        public async Task CollectCash_PerSessionRejectsWhenPreviousDebtRemains()
        {
            var (service, repository, enrollment, _) = CreateService();
            var member = TenantMember.Create(TenantId, StudentId, TenantRole.Student);
            enrollment.GetMemberAsync(TenantId, StudentId, Arg.Any<CancellationToken>()).Returns(member);
            repository.GetSessionInfoAsync(TenantId, StudentId, SessionId, Arg.Any<CancellationToken>()).Returns(SessionInfo(50m));
            repository.HasPresentAttendanceAsync(TenantId, StudentId, SessionId, Arg.Any<CancellationToken>()).Returns(true);
            repository.GetOutstandingObligationsAsync(TenantId, StudentId, Arg.Any<CancellationToken>())
                .Returns(new List<FinancialObligation>
                {
                    FinancialObligation.Create(TenantId, StudentId, GroupId, Guid.NewGuid(), "Older group", new DateTime(2026, 11, 17), 25m),
                    FinancialObligation.Create(TenantId, StudentId, GroupId, SessionId, "Math group", new DateTime(2026, 11, 18), 50m)
                });

            var result = await service.CollectCashAsync(TenantId, Guid.NewGuid(),
                new CashPaymentRequest(StudentId, SessionId, PaymentType.PerSession, 50m), CancellationToken.None);

            Assert.Equal("Finance.PreviousDebtMustBeSettled", result.Error.Code);
            repository.DidNotReceive().AddPayment(Arg.Any<Transaction>());
        }

        [Fact]
        public async Task CollectCash_DebtSettlementAllowsPartialPaymentAndLeavesAccurateBalance()
        {
            var (service, repository, enrollment, _) = CreateService();
            var member = TenantMember.Create(TenantId, StudentId, TenantRole.Student);
            member.DeductBalance(80m);
            var debt = FinancialObligation.Create(TenantId, StudentId, GroupId, Guid.NewGuid(), "Older group", new DateTime(2026, 11, 10), 80m);
            enrollment.GetMemberAsync(TenantId, StudentId, Arg.Any<CancellationToken>()).Returns(member);
            repository.GetSessionInfoAsync(TenantId, StudentId, SessionId, Arg.Any<CancellationToken>()).Returns(SessionInfo(50m));
            repository.HasPresentAttendanceAsync(TenantId, StudentId, SessionId, Arg.Any<CancellationToken>()).Returns(true);
            repository.GetOutstandingObligationsAsync(TenantId, StudentId, Arg.Any<CancellationToken>()).Returns(new List<FinancialObligation> { debt });

            var result = await service.CollectCashAsync(TenantId, Guid.NewGuid(),
                new CashPaymentRequest(StudentId, SessionId, PaymentType.DebtSettlement, 50m), CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(30m, result.Value!.RemainingDebt);
            Assert.Equal(30m, member.CurrentBalance * -1);
            Assert.Equal(30m, debt.OutstandingAmount);
            repository.Received(1).AddPayment(Arg.Is<Transaction>(x => x.Type == PaymentType.DebtSettlement && x.Amount == 50m));
        }

        [Fact]
        public async Task CollectCash_PerSessionOverpaymentBecomesWalletCredit()
        {
            var (service, repository, enrollment, _) = CreateService();
            var member = TenantMember.Create(TenantId, StudentId, TenantRole.Student);
            member.DeductBalance(50m);
            var today = FinancialObligation.Create(TenantId, StudentId, GroupId, SessionId, "Math group", new DateTime(2026, 11, 18), 50m);
            enrollment.GetMemberAsync(TenantId, StudentId, Arg.Any<CancellationToken>()).Returns(member);
            repository.GetSessionInfoAsync(TenantId, StudentId, SessionId, Arg.Any<CancellationToken>()).Returns(SessionInfo(50m));
            repository.HasPresentAttendanceAsync(TenantId, StudentId, SessionId, Arg.Any<CancellationToken>()).Returns(true);
            repository.GetOutstandingObligationsAsync(TenantId, StudentId, Arg.Any<CancellationToken>()).Returns(new List<FinancialObligation> { today });
            repository.GetObligationAsync(TenantId, StudentId, SessionId, Arg.Any<CancellationToken>()).Returns(today);

            var result = await service.CollectCashAsync(TenantId, Guid.NewGuid(),
                new CashPaymentRequest(StudentId, SessionId, PaymentType.PerSession, 70m), CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(0m, result.Value!.RemainingDebt);
            Assert.Equal(20m, member.CurrentBalance);
            Assert.True(today.IsSettled);
            repository.Received(1).AddPlatformCommission(Arg.Is<PlatformCommission>(x =>
                x.Type == PlatformCommissionType.PerSession && x.Amount == 1m
                && x.SessionId == SessionId && x.SourceTransactionId != Guid.Empty));
        }

        [Fact]
        public async Task CollectCash_MonthlySubscriptionCoversTodayAndKeepsOverpaymentAsCredit()
        {
            var (service, repository, enrollment, _) = CreateService();
            var member = TenantMember.Create(TenantId, StudentId, TenantRole.Student);
            member.DeductBalance(50m);
            var today = FinancialObligation.Create(TenantId, StudentId, GroupId, SessionId, "Math group", new DateTime(2026, 11, 18), 50m);
            repository.GetSessionInfoAsync(TenantId, StudentId, SessionId, Arg.Any<CancellationToken>())
                .Returns(SessionInfo(perSession: 50m, monthly: 500m));
            repository.HasPresentAttendanceAsync(TenantId, StudentId, SessionId, Arg.Any<CancellationToken>()).Returns(true);
            repository.GetOutstandingObligationsAsync(TenantId, StudentId, Arg.Any<CancellationToken>()).Returns(new List<FinancialObligation> { today });
            repository.GetObligationAsync(TenantId, StudentId, SessionId, Arg.Any<CancellationToken>()).Returns(today);
            repository.HasMonthlySubscriptionAsync(TenantId, StudentId, GroupId, 2026, 11, Arg.Any<CancellationToken>()).Returns(false);
            enrollment.GetMemberAsync(TenantId, StudentId, Arg.Any<CancellationToken>()).Returns(member);

            var result = await service.CollectCashAsync(TenantId, Guid.NewGuid(),
                new CashPaymentRequest(StudentId, SessionId, PaymentType.MonthlySubscription, 550m), CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(0m, today.OutstandingAmount);
            Assert.Equal(50m, member.CurrentBalance);
            repository.Received(1).AddSubscription(Arg.Is<MonthlySubscription>(x => x.Amount == 500m && x.Month == 11));
            repository.Received(1).AddPlatformCommission(Arg.Is<PlatformCommission>(x =>
                x.Type == PlatformCommissionType.MonthlySubscription && x.Amount == 5m
                && x.SubscriptionYear == 2026 && x.SubscriptionMonth == 11));
        }

        private static (FinancialLedgerService Service, IFinancialLedgerRepository Repository,
            IEnrollmentRepository Enrollment, IUnitOfWork UnitOfWork) CreateService()
        {
            var repository = Substitute.For<IFinancialLedgerRepository>();
            var enrollment = Substitute.For<IEnrollmentRepository>();
            var unitOfWork = Substitute.For<IUnitOfWork>();
            var audit = Substitute.For<IAuditRepository>();
            unitOfWork.ExecuteInTransactionAsync(Arg.Any<Func<Task<Result<CashPaymentResult>>>>(),
                    Arg.Any<IsolationLevel>(), Arg.Any<CancellationToken>())
                .Returns(call => call.Arg<Func<Task<Result<CashPaymentResult>>>>()());
            return (new FinancialLedgerService(repository, enrollment, unitOfWork, audit), repository, enrollment, unitOfWork);
        }

        private static FinancialSessionInfo SessionInfo(decimal? perSession, decimal? monthly = null) =>
            new(TenantId, StudentId, GroupId, SessionId, "Math group", "UTC", DateTime.SpecifyKind(new DateTime(2026, 11, 18), DateTimeKind.Utc), perSession, monthly);
    }
}
