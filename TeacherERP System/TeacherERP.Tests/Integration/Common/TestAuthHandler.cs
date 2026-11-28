using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TeacherERP.Application.Sessions;

namespace TeacherERP.Tests.Integration;

// مصادقة بديلة للاختبارات: الهوية والدور من Headers بدل JWT (السنتر نفسه يتحدد بـ X-Teacher-Id كالمعتاد)
public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "Test";
    public const string UserIdHeader = "X-Test-UserId";
    public const string RoleHeader = "X-Test-Role";
    public const string SessionIdHeader = "X-Test-SessionId";

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(UserIdHeader, out var userId) || string.IsNullOrWhiteSpace(userId))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var role = Request.Headers.TryGetValue(RoleHeader, out var roleValue) && !string.IsNullOrWhiteSpace(roleValue)
            ? roleValue.ToString()
            : "SuperAdmin";

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role)
        };

        if (Request.Headers.TryGetValue(SessionIdHeader, out var sessionIdValue)
            && Guid.TryParse(sessionIdValue.ToString(), out var sessionId))
        {
            claims.Add(new Claim(AuthClaimTypes.SessionId, sessionId.ToString()));
        }

        var identity = new ClaimsIdentity(claims, SchemeName);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
