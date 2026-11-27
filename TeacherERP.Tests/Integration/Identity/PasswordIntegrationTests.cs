using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using TeacherERP.Domain.Entities;
using TeacherERP.Domain.Shared;

namespace TeacherERP.Tests.Integration;

public sealed class PasswordIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public PasswordIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GoogleAccount_CanSetInitialPassword_AndThenChangeIt()
    {
        var user = await CreateUserAsync(withGoogleLogin: true, password: null);
        var client = AuthenticatedClient(user.Id);

        var setPassword = await client.PostAsJsonAsync(
            "api/Users/me/set-password", new { newPassword = "Google1" });
        Assert.Equal(HttpStatusCode.OK, setPassword.StatusCode);

        var changePassword = await client.PostAsJsonAsync(
            "api/Users/me/password", new { currentPassword = "Google1", newPassword = "Local2" });
        Assert.Equal(HttpStatusCode.OK, changePassword.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var storedUser = await userManager.FindByIdAsync(user.Id.ToString());
        Assert.NotNull(storedUser);
        Assert.True(await userManager.CheckPasswordAsync(storedUser, "Local2"));
        Assert.False(await userManager.CheckPasswordAsync(storedUser, "Google1"));
    }

    [Fact]
    public async Task SetPassword_RequiresLinkedGoogleLogin()
    {
        var user = await CreateUserAsync(withGoogleLogin: false, password: null);
        var response = await AuthenticatedClient(user.Id).PostAsJsonAsync(
            "api/Users/me/set-password", new { newPassword = "Google1" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Auth.GoogleLoginRequired", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task SetPassword_RejectsAccountThatAlreadyHasPassword()
    {
        var user = await CreateUserAsync(withGoogleLogin: true, password: "Current1");
        var response = await AuthenticatedClient(user.Id).PostAsJsonAsync(
            "api/Users/me/set-password", new { newPassword = "Google1" });

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Contains("Auth.PasswordAlreadySet", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task SetPassword_RequiresAuthentication()
    {
        var response = await _factory.CreateClient().PostAsJsonAsync(
            "api/Users/me/set-password", new { newPassword = "Google1" });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private HttpClient AuthenticatedClient(Guid userId)
    {
        var client = _factory.CreateClientFor(userId, role: "Student");
        client.DefaultRequestHeaders.Add(TestAuthHandler.SessionIdHeader, Guid.NewGuid().ToString());
        return client;
    }

    private async Task<ApplicationUser> CreateUserAsync(bool withGoogleLogin, string? password)
    {
        using var scope = _factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var user = ApplicationUser.CreateStudent($"A7 User {Guid.NewGuid():N}", $"a7-{Guid.NewGuid():N}@example.test");
        user.EmailConfirmed = true;

        var creation = password is null
            ? await userManager.CreateAsync(user)
            : await userManager.CreateAsync(user, password);
        Assert.True(creation.Succeeded, string.Join("; ", creation.Errors.Select(error => error.Description)));

        if (withGoogleLogin)
        {
            var login = await userManager.AddLoginAsync(
                user, new UserLoginInfo("Google", $"google-{user.Id:N}", "Google"));
            Assert.True(login.Succeeded, string.Join("; ", login.Errors.Select(error => error.Description)));
        }

        return user;
    }
}
