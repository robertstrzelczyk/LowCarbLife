using Microsoft.AspNetCore.Identity;

namespace LowCarbLife.Api.Features.Auth;

public class ApplicationUser : IdentityUser
{
    public string? DisplayName { get; set; }
}
