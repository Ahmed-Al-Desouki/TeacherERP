using System.Data;
using NSubstitute;
using TeacherERP.Application.Features.Team.RemoveAssistant;
using TeacherERP.Application.Features.Team.SetAssistantSuspension;
using TeacherERP.Application.Features.Team.SetFinancialDelegation;
using TeacherERP.Application.Features.Tenants;
using TeacherERP.Application.Interfaces;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Tests.Unit;

public sealed class AssistantAuditTests
{
    private static readonly Guid TenantId = Guid.NewGuid();
    private static readonly Guid ActorId = Guid.NewGuid();
    private static readonly Guid AssistantId = Guid.NewGuid();

    [Fact]
    public async Task SuspensionTransition_IsAuditedWithActorTenantAndAssistant()
    {
        var tenant = Substitute.For<ICurrentTenantAccessor>();
        tenant.Current.Returns(new TenantContext(TenantId, TenantRole.Owner, MembershipStatus.Active));
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.UserId.Returns(ActorId);
        var team = Substitute.For<ITeamRepository>();
        var member = TenantMember.Create(TenantId, AssistantId, TenantRole.Assistant);
        team.GetTrackedAssistantAsync(AssistantId, Arg.Any<CancellationToken>()).Returns(member);
        var audit = Substitute.For<IAuditRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var handler = new SetAssistantSuspensionCommandHandler(tenant, currentUser, team, audit, unitOfWork);
        var result = await handler.Handle(new SetAssistantSuspensionCommand(AssistantId, IsSuspended: true), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(MembershipStatus.Suspended, member.Status);
        audit.Received(1).Add(Arg.Is<AuditLog>(log =>
            log.TenantId == TenantId && log.ActorUserId == ActorId && log.TargetUserId == AssistantId &&
            log.Action == AuditAction.AssistantSuspended));
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RepeatingSuspension_DoesNotCreateDuplicateAuditEntry()
    {
        var tenant = Substitute.For<ICurrentTenantAccessor>();
        tenant.Current.Returns(new TenantContext(TenantId, TenantRole.Owner, MembershipStatus.Active));
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.UserId.Returns(ActorId);
        var team = Substitute.For<ITeamRepository>();
        var member = TenantMember.Create(TenantId, AssistantId, TenantRole.Assistant);
        member.Suspend();
        team.GetTrackedAssistantAsync(AssistantId, Arg.Any<CancellationToken>()).Returns(member);
        var audit = Substitute.For<IAuditRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var handler = new SetAssistantSuspensionCommandHandler(tenant, currentUser, team, audit, unitOfWork);
        var result = await handler.Handle(new SetAssistantSuspensionCommand(AssistantId, IsSuspended: true), CancellationToken.None);

        Assert.True(result.IsSuccess);
        audit.DidNotReceiveWithAnyArgs().Add(default!);
        await unitOfWork.DidNotReceiveWithAnyArgs().SaveChangesAsync(default);
    }

    [Fact]
    public async Task FinancialDelegationGrant_IsAudited()
    {
        var tenant = Substitute.For<ICurrentTenantAccessor>();
        tenant.Current.Returns(new TenantContext(TenantId, TenantRole.Owner, MembershipStatus.Active));
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.UserId.Returns(ActorId);
        var team = Substitute.For<ITeamRepository>();
        var member = TenantMember.Create(TenantId, AssistantId, TenantRole.Assistant);
        team.GetTrackedAssistantAsync(AssistantId, Arg.Any<CancellationToken>()).Returns(member);
        var audit = Substitute.For<IAuditRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();

        var handler = new SetFinancialDelegationCommandHandler(tenant, currentUser, team, audit, unitOfWork);
        var result = await handler.Handle(new SetFinancialDelegationCommand(AssistantId, Granted: true), CancellationToken.None);

        Assert.True(result.IsSuccess);
        audit.Received(1).Add(Arg.Is<AuditLog>(log =>
            log.TenantId == TenantId && log.ActorUserId == ActorId && log.TargetUserId == AssistantId &&
            log.Action == AuditAction.FinancialDelegationGranted));
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Removal_IsAuditedInsideTheMembershipTransaction()
    {
        var tenant = Substitute.For<ICurrentTenantAccessor>();
        tenant.Current.Returns(new TenantContext(TenantId, TenantRole.Owner, MembershipStatus.Active));
        var currentUser = Substitute.For<ICurrentUserService>();
        currentUser.UserId.Returns(ActorId);
        var team = Substitute.For<ITeamRepository>();
        var member = TenantMember.Create(TenantId, AssistantId, TenantRole.Assistant);
        team.GetTrackedAssistantAsync(AssistantId, Arg.Any<CancellationToken>()).Returns(member);
        var users = Substitute.For<IUserRepository>();
        users.GetByIdAsync(AssistantId, Arg.Any<CancellationToken>()).Returns((ApplicationUser?)null);
        var accountService = Substitute.For<IUserAccountService>();
        var audit = Substitute.For<IAuditRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        unitOfWork.ExecuteInTransactionAsync(Arg.Any<Func<Task<TeacherERP.Domain.Shared.Result<bool>>>>(),
                Arg.Any<IsolationLevel>(), Arg.Any<CancellationToken>())
            .Returns(call => call.Arg<Func<Task<TeacherERP.Domain.Shared.Result<bool>>>>()());

        var handler = new RemoveAssistantCommandHandler(tenant, currentUser, team, audit, users, accountService, unitOfWork);
        var result = await handler.Handle(new RemoveAssistantCommand(AssistantId), CancellationToken.None);

        Assert.True(result.IsSuccess);
        audit.Received(1).Add(Arg.Is<AuditLog>(log =>
            log.TenantId == TenantId && log.ActorUserId == ActorId && log.TargetUserId == AssistantId &&
            log.Action == AuditAction.AssistantRemoved));
        team.Received(1).RemoveMember(member);
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
