using LowCarbLife.Api.Features.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace LowCarbLife.Api.Tests.TestSupport;

internal static class TestFactories
{
    public const string JwtKey = "unit-test-jwt-key-with-at-least-32-characters!";
    public const string JwtIssuer = "LowCarbLife.Tests";
    public const string JwtAudience = "LowCarbLife.Tests.Audience";

    public static IConfiguration Configuration(string? expiresInMinutes = "60") =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = JwtKey,
                ["Jwt:Issuer"] = JwtIssuer,
                ["Jwt:Audience"] = JwtAudience,
                ["Jwt:ExpiresInMinutes"] = expiresInMinutes
            })
            .Build();

    public static UserManager<ApplicationUser> UserManager() =>
        Substitute.For<UserManager<ApplicationUser>>(
            Substitute.For<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);

    public static T WithHttpContext<T>(this T controller) where T : ControllerBase
    {
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        return controller;
    }

    public static ApplicationUser User(
        string id = "user-1",
        string email = "jan@example.com",
        string? displayName = "Jan") =>
        new()
        {
            Id = id,
            UserName = email,
            Email = email,
            DisplayName = displayName
        };
}
