using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LowCarbLife.Api.Features.Auth;
using LowCarbLife.Api.Tests.TestSupport;
using Microsoft.IdentityModel.Tokens;

namespace LowCarbLife.Api.Tests.Auth;

public class TokenServiceTests
{
    private static TokenValidationParameters ValidationParameters(string key = TestFactories.JwtKey) =>
        new()
        {
            ValidateIssuer = true,
            ValidIssuer = TestFactories.JwtIssuer,
            ValidateAudience = true,
            ValidAudience = TestFactories.JwtAudience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ClockSkew = TimeSpan.Zero
        };

    private static ClaimsPrincipal Validate(string token, string key = TestFactories.JwtKey) =>
        new JwtSecurityTokenHandler().ValidateToken(token, ValidationParameters(key), out _);

    [Fact]
    public void CreateToken_ProducesTokenValidForConfiguredIssuerAndAudience()
    {
        var service = new TokenService(TestFactories.Configuration());

        var token = service.CreateToken(TestFactories.User(), [Roles.User]);

        var principal = Validate(token);
        Assert.True(principal.Identity?.IsAuthenticated);
    }

    [Fact]
    public void CreateToken_ContainsUserIdentityClaims()
    {
        var service = new TokenService(TestFactories.Configuration());
        var user = TestFactories.User(id: "abc-123", email: "anna@example.com");

        var principal = Validate(service.CreateToken(user, [Roles.User]));

        Assert.Equal("abc-123", principal.FindFirstValue(ClaimTypes.NameIdentifier));
        Assert.Equal("anna@example.com", principal.FindFirstValue(ClaimTypes.Email));
        Assert.Equal("anna@example.com", principal.Identity!.Name);
    }

    [Fact]
    public void CreateToken_FallsBackToEmailWhenUserNameMissing()
    {
        var service = new TokenService(TestFactories.Configuration());
        var user = TestFactories.User(email: "bez-nazwy@example.com");
        user.UserName = null;

        var principal = Validate(service.CreateToken(user, []));

        Assert.Equal("bez-nazwy@example.com", principal.Identity!.Name);
    }

    [Fact]
    public void CreateToken_AddsAllRolesAsClaims()
    {
        var service = new TokenService(TestFactories.Configuration());

        var principal = Validate(service.CreateToken(TestFactories.User(), [Roles.Admin, Roles.User]));

        Assert.True(principal.IsInRole(Roles.Admin));
        Assert.True(principal.IsInRole(Roles.User));
    }

    [Fact]
    public void CreateToken_WithoutRoles_IsNotAdmin()
    {
        var service = new TokenService(TestFactories.Configuration());

        var principal = Validate(service.CreateToken(TestFactories.User(), []));

        Assert.False(principal.IsInRole(Roles.Admin));
    }

    [Fact]
    public void CreateToken_UsesConfiguredLifetime()
    {
        var service = new TokenService(TestFactories.Configuration("30"));

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(service.CreateToken(TestFactories.User(), []));

        var lifetime = jwt.ValidTo - DateTime.UtcNow;
        Assert.InRange(lifetime, TimeSpan.FromMinutes(29), TimeSpan.FromMinutes(30));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("nie-liczba")]
    public void CreateToken_InvalidLifetime_DefaultsToEightHours(string? configured)
    {
        var service = new TokenService(TestFactories.Configuration(configured));

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(service.CreateToken(TestFactories.User(), []));

        var lifetime = jwt.ValidTo - DateTime.UtcNow;
        Assert.InRange(lifetime, TimeSpan.FromMinutes(479), TimeSpan.FromMinutes(480));
    }

    [Fact]
    public void CreateToken_IsRejectedWhenSignedWithDifferentKey()
    {
        var service = new TokenService(TestFactories.Configuration());
        var token = service.CreateToken(TestFactories.User(), [Roles.Admin]);

        Assert.ThrowsAny<SecurityTokenException>(
            () => Validate(token, "a-completely-different-key-of-32-chars!!"));
    }
}
