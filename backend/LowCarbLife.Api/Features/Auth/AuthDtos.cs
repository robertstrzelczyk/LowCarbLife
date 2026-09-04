namespace LowCarbLife.Api.Features.Auth;

public record RegisterRequest(string Email, string Password, string? DisplayName);

public record LoginRequest(string Email, string Password);

public record AuthResponse(string Token, string Email, string? DisplayName, bool IsAdmin);

public record MeResponse(string Email, string? DisplayName, bool IsAdmin);
