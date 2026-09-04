namespace LowCarbLife.Api.Features.Contact;

public record SendContactRequest(string Name, string Email, string Message);

public record ContactMessageDto(Guid Id, string Name, string Email, string Message, DateTime CreatedAt);
