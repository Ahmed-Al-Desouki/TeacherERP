using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using TeacherERP.API.Security;
using TeacherERP.Application.Interfaces;
using TeacherERP.Application.Sessions;
using TeacherERP.Domain.Entities;

namespace TeacherERP.Tests.Integration.Sessions;

public sealed class SessionJwtRevocationIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public SessionJwtRevocationIntegrationTests(CustomWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task ValidatedJwt_IsRejectedAfterItsSessionIsRevoked()
    {
        var now = DateTime.UtcNow;
        var user = ApplicationUser.CreateStudent("Session revocation", $"session-revoke-{Guid.NewGuid():N}@example.test");
        var session = LoginSession.Create(
            user.Id,
            $"device-{Guid.NewGuid():N}",
            "Revocation integration test",
            TokenHasher.Hash(Guid.NewGuid().ToString("N")),
            now,
            SessionPolicy.RefreshLifetime,
            SessionPolicy.AbsoluteLifetime);

        await _factory.SeedAsync(async db =>
        {
            db.Users.Add(user);
            db.LoginSessions.Add(session);
            await db.SaveChangesAsync();
        });

        using var scope = _factory.Services.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<ISessionManager>();
        var sessionEvents = new SessionJwtBearerEvents();

        var beforeRevoke = CreateValidatedContext(scope.ServiceProvider, session.Id);
        await sessionEvents.TokenValidated(beforeRevoke);
        Assert.Null(beforeRevoke.Result);

        var revoked = await manager.RevokeSessionAsync(user.Id, session.Id, SessionRevokeReason.UserLogout, CancellationToken.None);
        Assert.True(revoked.IsSuccess);

        var afterRevoke = CreateValidatedContext(scope.ServiceProvider, session.Id);
        await sessionEvents.TokenValidated(afterRevoke);
        Assert.NotNull(afterRevoke.Result?.Failure);
    }

    private static TokenValidatedContext CreateValidatedContext(IServiceProvider services, Guid sessionId)
    {
        var httpContext = new DefaultHttpContext { RequestServices = services };
        var scheme = new AuthenticationScheme(JwtBearerDefaults.AuthenticationScheme, JwtBearerDefaults.AuthenticationScheme, typeof(JwtBearerHandler));
        var context = new TokenValidatedContext(httpContext, scheme, new JwtBearerOptions());
        context.Principal = new ClaimsPrincipal(new ClaimsIdentity(
            new[] { new Claim(AuthClaimTypes.SessionId, sessionId.ToString()) },
            JwtBearerDefaults.AuthenticationScheme));
        return context;
    }
}
