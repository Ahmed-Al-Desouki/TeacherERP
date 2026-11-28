using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TeacherERP.Application.Sessions;
using TeacherERP.Infrastructure.Services;

namespace TeacherERP.Tests.Unit.Identity;

public sealed class CurrentUserTimeZoneTests
{
    [Fact]
    public void ReadsThePersistedTimeZoneFromTheAuthenticatedUserClaims()
    {
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(AuthClaimTypes.TimeZoneId, "Asia/Riyadh")], "test"));
        var accessor = new HttpContextAccessor { HttpContext = context };
        var currentUser = new CurrentUserService(accessor);

        Assert.Equal("Asia/Riyadh", currentUser.TimeZoneId);
    }

    [Fact]
    public void ReturnsNullForTokensIssuedBeforeTimeZoneClaimsWereAdded()
    {
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity([], "test"));
        var accessor = new HttpContextAccessor { HttpContext = context };
        var currentUser = new CurrentUserService(accessor);

        Assert.Null(currentUser.TimeZoneId);
    }

    [Fact]
    public void IgnoresInvalidTimeZoneClaims()
    {
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(AuthClaimTypes.TimeZoneId, "Invalid/Zone")], "test"));
        var accessor = new HttpContextAccessor { HttpContext = context };
        var currentUser = new CurrentUserService(accessor);

        Assert.Null(currentUser.TimeZoneId);
    }

    [Fact]
    public void TimeZoneAccessorFallsBackToThePersistedClaimAndAllowsRequestOverride()
    {
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity(
            [new Claim(AuthClaimTypes.TimeZoneId, "Asia/Riyadh")], "test"));
        var accessor = new HttpUserTimeZoneAccessor(new HttpContextAccessor { HttpContext = context });

        Assert.Equal("Asia/Riyadh", accessor.TimeZoneId);

        context.Request.Headers[HttpUserTimeZoneAccessor.HeaderName] = "Africa/Cairo";
        Assert.Equal("Africa/Cairo", accessor.TimeZoneId);

        context.Request.Headers[HttpUserTimeZoneAccessor.HeaderName] = "Invalid/Zone";
        Assert.Equal("Asia/Riyadh", accessor.TimeZoneId);
    }
}
