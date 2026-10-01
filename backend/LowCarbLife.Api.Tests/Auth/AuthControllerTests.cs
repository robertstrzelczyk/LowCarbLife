using System.Security.Claims;
using LowCarbLife.Api.Features.Auth;
using LowCarbLife.Api.Tests.TestSupport;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace LowCarbLife.Api.Tests.Auth;

public class AuthControllerTests
{
    private readonly UserManager<ApplicationUser> _userManager = TestFactories.UserManager();
    private readonly TokenService _tokenService = new(TestFactories.Configuration());

    private AuthController CreateController() =>
        new AuthController(_userManager, _tokenService).WithHttpContext();

    private void GivenRoles(params string[] roles) =>
        _userManager.GetRolesAsync(Arg.Any<ApplicationUser>()).Returns(roles.ToList());

    // ---------- Register ----------

    [Theory]
    [InlineData("", "Haslo123!")]
    [InlineData("   ", "Haslo123!")]
    [InlineData("jan@example.com", "")]
    [InlineData("jan@example.com", "   ")]
    public async Task Register_MissingEmailOrPassword_ReturnsBadRequest(string email, string password)
    {
        var result = await CreateController().Register(new RegisterRequest(email, password, null));

        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Email i hasło są wymagane.", bad.Value);
        await _userManager.DidNotReceiveWithAnyArgs().CreateAsync(default!, default!);
    }

    [Fact]
    public async Task Register_ValidRequest_CreatesTrimmedUserAndAssignsUserRole()
    {
        _userManager.CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>()).Returns(IdentityResult.Success);
        GivenRoles(Roles.User);

        var result = await CreateController().Register(
            new RegisterRequest("  jan@example.com ", "Haslo123!", "  Jan  "));

        var response = Assert.IsType<AuthResponse>(Assert.IsType<OkObjectResult>(result.Result).Value);
        Assert.False(string.IsNullOrWhiteSpace(response.Token));
        Assert.Equal("jan@example.com", response.Email);
        Assert.Equal("Jan", response.DisplayName);
        Assert.False(response.IsAdmin);

        await _userManager.Received(1).CreateAsync(
            Arg.Is<ApplicationUser>(u =>
                u.Email == "jan@example.com" &&
                u.UserName == "jan@example.com" &&
                u.DisplayName == "Jan"),
            "Haslo123!");
        await _userManager.Received(1).AddToRoleAsync(
            Arg.Is<ApplicationUser>(u => u.Email == "jan@example.com"),
            Roles.User);
    }

    [Fact]
    public async Task Register_BlankDisplayName_IsStoredAsNull()
    {
        _userManager.CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>()).Returns(IdentityResult.Success);
        GivenRoles(Roles.User);

        var result = await CreateController().Register(new RegisterRequest("jan@example.com", "Haslo123!", "   "));

        var response = Assert.IsType<AuthResponse>(Assert.IsType<OkObjectResult>(result.Result).Value);
        Assert.Null(response.DisplayName);
    }

    [Fact]
    public async Task Register_IdentityFailure_ReturnsJoinedErrorDescriptions()
    {
        _userManager.CreateAsync(Arg.Any<ApplicationUser>(), Arg.Any<string>()).Returns(
            IdentityResult.Failed(
                new IdentityError { Code = "A", Description = "Hasło za krótkie." },
                new IdentityError { Code = "B", Description = "Email zajęty." }));

        var result = await CreateController().Register(new RegisterRequest("jan@example.com", "x", null));

        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Hasło za krótkie. Email zajęty.", bad.Value);
        await _userManager.DidNotReceiveWithAnyArgs().AddToRoleAsync(default!, default!);
    }

    // ---------- Login ----------

    [Fact]
    public async Task Login_UnknownEmail_ReturnsUnauthorized()
    {
        _userManager.FindByEmailAsync(Arg.Any<string>()).Returns((ApplicationUser?)null);

        var result = await CreateController().Login(new LoginRequest("nikt@example.com", "Haslo123!"));

        var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result.Result);
        Assert.Equal("Nieprawidłowy email lub hasło.", unauthorized.Value);
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsUnauthorized()
    {
        var user = TestFactories.User();
        _userManager.FindByEmailAsync("jan@example.com").Returns(user);
        _userManager.CheckPasswordAsync(user, "zle").Returns(false);

        var result = await CreateController().Login(new LoginRequest("jan@example.com", "zle"));

        Assert.IsType<UnauthorizedObjectResult>(result.Result);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsTokenForTrimmedEmail()
    {
        var user = TestFactories.User();
        _userManager.FindByEmailAsync("jan@example.com").Returns(user);
        _userManager.CheckPasswordAsync(user, "Haslo123!").Returns(true);
        GivenRoles(Roles.User);

        var result = await CreateController().Login(new LoginRequest("  jan@example.com  ", "Haslo123!"));

        var response = Assert.IsType<AuthResponse>(Assert.IsType<OkObjectResult>(result.Result).Value);
        Assert.False(string.IsNullOrWhiteSpace(response.Token));
        Assert.Equal("jan@example.com", response.Email);
        Assert.Equal("Jan", response.DisplayName);
        Assert.False(response.IsAdmin);
    }

    [Fact]
    public async Task Login_AdminUser_IsMarkedAsAdmin()
    {
        var user = TestFactories.User(email: "admin@example.com", displayName: "Admin");
        _userManager.FindByEmailAsync("admin@example.com").Returns(user);
        _userManager.CheckPasswordAsync(user, "Admin123!").Returns(true);
        GivenRoles(Roles.Admin);

        var result = await CreateController().Login(new LoginRequest("admin@example.com", "Admin123!"));

        var response = Assert.IsType<AuthResponse>(Assert.IsType<OkObjectResult>(result.Result).Value);
        Assert.True(response.IsAdmin);
    }

    // ---------- Me ----------

    [Fact]
    public async Task Me_NoCurrentUser_ReturnsUnauthorized()
    {
        _userManager.GetUserAsync(Arg.Any<ClaimsPrincipal>()).Returns((ApplicationUser?)null);

        var result = await CreateController().Me();

        Assert.IsType<UnauthorizedResult>(result.Result);
    }

    [Theory]
    [InlineData(Roles.Admin, true)]
    [InlineData(Roles.User, false)]
    public async Task Me_ReturnsProfileWithAdminFlagFromRoles(string role, bool expectedAdmin)
    {
        _userManager.GetUserAsync(Arg.Any<ClaimsPrincipal>())
            .Returns(TestFactories.User(email: "ola@example.com", displayName: "Ola"));
        GivenRoles(role);

        var result = await CreateController().Me();

        var me = Assert.IsType<MeResponse>(Assert.IsType<OkObjectResult>(result.Result).Value);
        Assert.Equal("ola@example.com", me.Email);
        Assert.Equal("Ola", me.DisplayName);
        Assert.Equal(expectedAdmin, me.IsAdmin);
    }
}
