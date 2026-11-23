using TeacherERP.Domain.Entities;
using TeacherERP.Application.Features.Centers.CreateCenter;
using TeacherERP.Application.Features.Centers;

namespace TeacherERP.Tests.Unit;

public sealed class CenterMembershipTests
{
    [Fact]
    public void LocationPolicyBlocksAnotherOwnedCenterWithinFiftyMeters_ButExcludesCenterBeingEdited()
    {
        var existing = Center.Create("Existing", Guid.NewGuid(), 30.0561, 31.3301);

        Assert.True(CenterLocationPolicy.HasNearbyOwnedCenter(
            new[] { existing }, 30.0561, 31.3301));
        Assert.False(CenterLocationPolicy.HasNearbyOwnedCenter(
            new[] { existing }, 30.0561, 31.3301, existing.Id));
        Assert.False(CenterLocationPolicy.HasNearbyOwnedCenter(
            new[] { existing }, 30.0571, 31.3311));
    }

    [Fact]
    public void CenterCreationRequiresValidGeolocation()
    {
        var validator = new CreateMyCenterValidator();

        Assert.False(validator.Validate(new CreateMyCenterCommand("Center", null, null, null)).IsValid);
        Assert.True(validator.Validate(new CreateMyCenterCommand("Center", "Cairo", 30.05, 31.33)).IsValid);
        Assert.False(validator.Validate(new CreateMyCenterCommand("Center", "Cairo", 95, 31.33)).IsValid);
    }

    [Fact]
    public void JoinRequestAllowsReminderOnlyAfterTwentyFourHours_AndCancellationClosesIt()
    {
        var createdAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var request = CenterJoinRequest.Create(Guid.NewGuid(), Guid.NewGuid(), createdAt);

        Assert.False(request.CanResendAt(createdAt.AddHours(23), TimeSpan.FromHours(24)));
        Assert.True(request.CanResendAt(createdAt.AddHours(24), TimeSpan.FromHours(24)));

        request.Cancel(createdAt.AddHours(25));
        Assert.False(request.CanResendAt(createdAt.AddDays(2), TimeSpan.FromHours(24)));
        Assert.Equal(CenterJoinRequestStatus.Cancelled, request.Status);
    }

    [Fact]
    public void InvitationCanBeAcceptedOnce_AndExpiresAtDeadline()
    {
        var now = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var invitation = CenterInvitation.Create(Guid.NewGuid(), Guid.NewGuid(), new string('a', 64), now.AddHours(72));

        Assert.False(invitation.IsExpired(now.AddHours(71)));
        Assert.True(invitation.IsExpired(now.AddHours(72)));

        invitation.Accept(Guid.NewGuid(), now.AddHours(1));
        Assert.Equal(CenterInvitationStatus.Accepted, invitation.Status);
        Assert.False(invitation.IsExpired(now.AddDays(4)));
    }
}
