using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using TeacherERP.Domain.Entities;
using Xunit.Abstractions;

namespace TeacherERP.Tests.Integration;

public sealed class CenterMembershipIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly ITestOutputHelper _output;

    public CenterMembershipIntegrationTests(CustomWebApplicationFactory factory, ITestOutputHelper output)
    {
        _factory = factory;
        _output = output;
    }

    [Fact]
    public async Task FullA5HttpFlow_EmitsRequestResponseTrace()
    {
        SeededTeacher applicant = null!;
        SeededTeacher firstOwner = null!;
        SeededTeacher secondOwner = null!;
        SeededTeacher thirdOwner = null!;
        Guid resendRequestId = default;
        await _factory.SeedAsync(async db =>
        {
            applicant = await TestData.CreateTeacherAsync(db);
            firstOwner = await TestData.CreateTeacherAsync(db);
            secondOwner = await TestData.CreateTeacherAsync(db);
            thirdOwner = await TestData.CreateTeacherAsync(db);
            var oldRequest = CenterJoinRequest.Create(secondOwner.CenterId, applicant.TenantId, DateTime.UtcNow.AddHours(-25));
            resendRequestId = oldRequest.Id;
            db.CenterJoinRequests.Add(oldRequest);
            await db.SaveChangesAsync();
        });

        var applicantClient = _factory.CreateClientFor(applicant.OwnerId);
        var firstOwnerClient = _factory.CreateClientFor(firstOwner.OwnerId);
        var secondOwnerClient = _factory.CreateClientFor(secondOwner.OwnerId);
        var thirdOwnerClient = _factory.CreateClientFor(thirdOwner.OwnerId);

        async Task<HttpResponseMessage> Trace(string method, string path, HttpResponseMessage response)
        {
            var body = await response.Content.ReadAsStringAsync();
            _output.WriteLine($"{method} http://localhost{path}\n-> {(int)response.StatusCode} {response.StatusCode}\n<- {body}\n");
            Assert.True(response.IsSuccessStatusCode, $"{method} {path} failed: {body}");
            return response;
        }

        var created = await Trace("POST", "/api/Centers", await applicantClient.PostAsJsonAsync("api/Centers", new
        {
            name = "A5 Trace Center", address = "Cairo", latitude = 30.2, longitude = 31.6, geofenceRadiusMeters = 50
        }));
        var createdBody = await created.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        var createdCenterId = createdBody.GetProperty("id").GetGuid();

        await Trace("PUT", $"/api/Centers/{createdCenterId}", await applicantClient.PutAsJsonAsync(
            $"api/Centers/{createdCenterId}", new
            {
                name = "A5 Trace Center Updated", address = "New Cairo", latitude = 30.21, longitude = 31.61,
                geofenceRadiusMeters = 75
            }));

        var submitted = await Trace("POST", $"/api/Centers/{firstOwner.CenterId}/join-requests",
            await applicantClient.PostAsync($"api/Centers/{firstOwner.CenterId}/join-requests", null));
        var submittedBody = await submitted.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        var submittedRequestId = submittedBody.GetProperty("id").GetGuid();

        await Trace("GET", "/api/Centers/join-requests/mine", await applicantClient.GetAsync("api/Centers/join-requests/mine"));
        await Trace("GET", "/api/Centers/join-requests/received?status=Pending",
            await firstOwnerClient.GetAsync("api/Centers/join-requests/received?status=Pending"));
        await Trace("POST", $"/api/Centers/join-requests/{submittedRequestId}/approve",
            await firstOwnerClient.PostAsync($"api/Centers/join-requests/{submittedRequestId}/approve", null));

        var rejectedSubmission = await Trace("POST", $"/api/Centers/{thirdOwner.CenterId}/join-requests",
            await applicantClient.PostAsync($"api/Centers/{thirdOwner.CenterId}/join-requests", null));
        var rejectedBody = await rejectedSubmission.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        var rejectedRequestId = rejectedBody.GetProperty("id").GetGuid();
        await Trace("POST", $"/api/Centers/join-requests/{rejectedRequestId}/reject",
            await thirdOwnerClient.PostAsJsonAsync($"api/Centers/join-requests/{rejectedRequestId}/reject", new { reason = "Trace rejection" }));

        await Trace("POST", $"/api/Centers/join-requests/{resendRequestId}/resend",
            await applicantClient.PostAsync($"api/Centers/join-requests/{resendRequestId}/resend", null));
        await Trace("DELETE", $"/api/Centers/join-requests/{resendRequestId}",
            await applicantClient.DeleteAsync($"api/Centers/join-requests/{resendRequestId}"));

        var invitationResponse = await Trace("POST", $"/api/Centers/{secondOwner.CenterId}/invitations",
            await secondOwnerClient.PostAsync($"api/Centers/{secondOwner.CenterId}/invitations", null));
        var invitationBody = await invitationResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        var token = invitationBody.GetProperty("token").GetString()!;
        await Trace("POST", "/api/Centers/invitations/accept",
            await applicantClient.PostAsJsonAsync("api/Centers/invitations/accept", new { token }));
    }

    [Fact]
    public async Task NonTeacherCannotUseCenterMembershipEndpoints()
    {
        var response = await _factory.CreateClientFor(Guid.NewGuid(), role: "Student")
            .PostAsync($"api/Centers/{Guid.NewGuid()}/join-requests", null);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task TeacherCanRequestToJoinAndCenterOwnerCanApprove()
    {
        SeededTeacher applicant = null!;
        SeededTeacher owner = null!;
        await _factory.SeedAsync(async db =>
        {
            applicant = await TestData.CreateTeacherAsync(db);
            owner = await TestData.CreateTeacherAsync(db);
        });

        var requestResponse = await _factory.CreateClientFor(applicant.OwnerId)
            .PostAsync($"api/Centers/{owner.CenterId}/join-requests", null);
        Assert.Equal(HttpStatusCode.OK, requestResponse.StatusCode);
        var requestJson = await requestResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        var requestId = requestJson.GetProperty("id").GetGuid();

        var ownerClient = _factory.CreateClientFor(owner.OwnerId);
        var received = await ownerClient.GetAsync("api/Centers/join-requests/received?status=Pending");
        Assert.Equal(HttpStatusCode.OK, received.StatusCode);
        Assert.Contains(applicant.TenantId.ToString(), await received.Content.ReadAsStringAsync());

        var approval = await ownerClient.PostAsync($"api/Centers/join-requests/{requestId}/approve", null);
        Assert.Equal(HttpStatusCode.OK, approval.StatusCode);
        Assert.True(await _factory.QueryAsync(db => db.CenterTeachers.AnyAsync(link =>
            link.CenterId == owner.CenterId && link.TenantId == applicant.TenantId
            && link.Status == CenterTeacherStatus.Active && link.Origin == CenterTeacherOrigin.Request)));
    }

    [Fact]
    public async Task TeacherCanHaveOnlyThreePendingRequests_AndCancellationFreesASlot()
    {
        SeededTeacher applicant = null!;
        var centers = new List<SeededTeacher>();
        await _factory.SeedAsync(async db =>
        {
            applicant = await TestData.CreateTeacherAsync(db);
            for (var i = 0; i < 4; i++)
                centers.Add(await TestData.CreateTeacherAsync(db));
        });

        var client = _factory.CreateClientFor(applicant.OwnerId);
        var pendingIds = new List<Guid>();
        foreach (var target in centers.Take(3))
        {
            var response = await client.PostAsync($"api/Centers/{target.CenterId}/join-requests", null);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
            pendingIds.Add(body.GetProperty("id").GetGuid());
        }

        var overLimit = await client.PostAsync($"api/Centers/{centers[3].CenterId}/join-requests", null);
        Assert.Equal(HttpStatusCode.BadRequest, overLimit.StatusCode);
        Assert.Contains("Center.JoinRequestLimitReached", await overLimit.Content.ReadAsStringAsync());

        var cancelled = await client.DeleteAsync($"api/Centers/join-requests/{pendingIds[0]}");
        Assert.Equal(HttpStatusCode.OK, cancelled.StatusCode);
        var afterCancel = await client.PostAsync($"api/Centers/{centers[3].CenterId}/join-requests", null);
        Assert.Equal(HttpStatusCode.OK, afterCancel.StatusCode);
    }

    [Fact]
    public async Task TeacherCanCreateCenterOnlyWithCoordinates_AndUpdateItsLocation()
    {
        SeededTeacher teacher = null!;
        SeededTeacher otherTeacher = null!;
        await _factory.SeedAsync(async db =>
        {
            teacher = await TestData.CreateTeacherAsync(db);
            otherTeacher = await TestData.CreateTeacherAsync(db);
        });
        var client = _factory.CreateClientFor(teacher.OwnerId);

        var missingLocation = await client.PostAsJsonAsync("api/Centers", new { name = "New Center" });
        Assert.Equal(HttpStatusCode.BadRequest, missingLocation.StatusCode);

        var sameLocation = await client.PostAsJsonAsync("api/Centers", new
        {
            name = "Duplicate Location", latitude = 30.0561, longitude = 31.3301,
            geofenceRadiusMeters = 50
        });
        Assert.Equal(HttpStatusCode.BadRequest, sameLocation.StatusCode);
        Assert.Contains("Center.LocationAlreadyOwned", await sameLocation.Content.ReadAsStringAsync());

        var created = await client.PostAsJsonAsync("api/Centers", new
        {
            name = "Second Center", address = "Cairo", latitude = 30.2, longitude = 31.6,
            geofenceRadiusMeters = 50
        });
        Assert.Equal(HttpStatusCode.OK, created.StatusCode);
        var centerJson = await created.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        var centerId = centerJson.GetProperty("id").GetGuid();

        var sameLocationForAnotherTeacher = await _factory.CreateClientFor(otherTeacher.OwnerId)
            .PostAsJsonAsync("api/Centers", new
            {
                name = "Other Teacher Center", address = "Cairo", latitude = 30.2, longitude = 31.6,
                geofenceRadiusMeters = 50
            });
        Assert.Equal(HttpStatusCode.OK, sameLocationForAnotherTeacher.StatusCode);

        var updateToOwnCenterLocation = await client.PutAsJsonAsync($"api/Centers/{centerId}", new
        {
            name = "Second Center", address = "Nasr City", latitude = 30.0561, longitude = 31.3301,
            geofenceRadiusMeters = 50
        });
        Assert.Equal(HttpStatusCode.BadRequest, updateToOwnCenterLocation.StatusCode);
        Assert.Contains("Center.LocationAlreadyOwned", await updateToOwnCenterLocation.Content.ReadAsStringAsync());
        Assert.True(await _factory.QueryAsync(db => db.Centers.AnyAsync(c =>
            c.Id == centerId && c.Latitude == 30.2 && c.Longitude == 31.6)));

        var update = await client.PutAsJsonAsync($"api/Centers/{centerId}", new
        {
            name = "Second Center Updated", address = "New Cairo", latitude = 30.21, longitude = 31.61,
            geofenceRadiusMeters = 75
        });
        Assert.Equal(HttpStatusCode.OK, update.StatusCode);
        Assert.True(await _factory.QueryAsync(db => db.Centers.AnyAsync(c =>
            c.Id == centerId && c.Latitude == 30.21 && c.Longitude == 31.61 && c.GeofenceRadiusMeters == 75)));
    }

    [Fact]
    public async Task OwnerCanRejectJoinRequest_AndApplicantCanSeeDecision()
    {
        SeededTeacher applicant = null!;
        SeededTeacher owner = null!;
        await _factory.SeedAsync(async db =>
        {
            applicant = await TestData.CreateTeacherAsync(db);
            owner = await TestData.CreateTeacherAsync(db);
        });

        var submission = await _factory.CreateClientFor(applicant.OwnerId)
            .PostAsync($"api/Centers/{owner.CenterId}/join-requests", null);
        var submitted = await submission.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        var requestId = submitted.GetProperty("id").GetGuid();

        var rejection = await _factory.CreateClientFor(owner.OwnerId)
            .PostAsJsonAsync($"api/Centers/join-requests/{requestId}/reject", new { reason = "Subject mismatch" });
        Assert.Equal(HttpStatusCode.OK, rejection.StatusCode);

        var mine = await _factory.CreateClientFor(applicant.OwnerId).GetAsync("api/Centers/join-requests/mine");
        Assert.Equal(HttpStatusCode.OK, mine.StatusCode);
        var body = await mine.Content.ReadAsStringAsync();
        Assert.Contains("Rejected", body);
        Assert.Contains("Subject mismatch", body);
    }

    [Fact]
    public async Task ReminderEndpointEnforcesTwentyFourHours()
    {
        SeededTeacher applicant = null!;
        SeededTeacher owner = null!;
        Guid requestId = default;
        await _factory.SeedAsync(async db =>
        {
            applicant = await TestData.CreateTeacherAsync(db);
            owner = await TestData.CreateTeacherAsync(db);
            var request = CenterJoinRequest.Create(owner.CenterId, applicant.TenantId, DateTime.UtcNow.AddHours(-25));
            requestId = request.Id;
            db.CenterJoinRequests.Add(request);
            await db.SaveChangesAsync();
        });

        var client = _factory.CreateClientFor(applicant.OwnerId);
        var first = await client.PostAsync($"api/Centers/join-requests/{requestId}/resend", null);
        var second = await client.PostAsync($"api/Centers/join-requests/{requestId}/resend", null);

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, second.StatusCode);
        Assert.Contains("Center.JoinRequestResendTooSoon", await second.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task InvitationActivatesMembershipAndCannotBeReused()
    {
        SeededTeacher applicant = null!;
        SeededTeacher owner = null!;
        await _factory.SeedAsync(async db =>
        {
            applicant = await TestData.CreateTeacherAsync(db);
            owner = await TestData.CreateTeacherAsync(db);
        });

        var created = await _factory.CreateClientFor(owner.OwnerId)
            .PostAsync($"api/Centers/{owner.CenterId}/invitations", null);
        Assert.Equal(HttpStatusCode.OK, created.StatusCode);
        var invitation = await created.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        var token = invitation.GetProperty("token").GetString()!;

        var applicantClient = _factory.CreateClientFor(applicant.OwnerId);
        var accepted = await applicantClient.PostAsJsonAsync("api/Centers/invitations/accept", new { token });
        Assert.Equal(HttpStatusCode.OK, accepted.StatusCode);
        Assert.True(await _factory.QueryAsync(db => db.CenterTeachers.AnyAsync(link =>
            link.CenterId == owner.CenterId && link.TenantId == applicant.TenantId
            && link.Status == CenterTeacherStatus.Active && link.Origin == CenterTeacherOrigin.Invitation)));

        var replay = await applicantClient.PostAsJsonAsync("api/Centers/invitations/accept", new { token });
        Assert.Equal(HttpStatusCode.BadRequest, replay.StatusCode);
    }
}
