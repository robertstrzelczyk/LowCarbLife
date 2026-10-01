using LowCarbLife.Api.Features.Contact;
using LowCarbLife.Api.Tests.TestSupport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LowCarbLife.Api.Tests.Contact;

public class ContactControllerTests
{
    private readonly TestDb _db = new();

    private ContactController CreateController() => new(_db.NewContext());

    [Fact]
    public async Task Send_ValidRequest_StoresTrimmedMessage()
    {
        var before = DateTime.UtcNow;

        var result = await CreateController().Send(
            new SendContactRequest("  Jan  ", "  jan@example.com ", "  Cześć!  "));

        Assert.IsType<OkResult>(result);
        await using var db = _db.NewContext();
        var stored = await db.ContactMessages.SingleAsync();
        Assert.Equal("Jan", stored.Name);
        Assert.Equal("jan@example.com", stored.Email);
        Assert.Equal("Cześć!", stored.Message);
        Assert.NotEqual(Guid.Empty, stored.Id);
        Assert.True(stored.CreatedAt >= before);
    }

    [Theory]
    [InlineData("", "jan@example.com", "Wiadomość")]
    [InlineData("   ", "jan@example.com", "Wiadomość")]
    [InlineData("Jan", "", "Wiadomość")]
    [InlineData("Jan", "   ", "Wiadomość")]
    [InlineData("Jan", "jan@example.com", "")]
    [InlineData("Jan", "jan@example.com", "   ")]
    public async Task Send_MissingField_ReturnsBadRequestAndDoesNotSave(string name, string email, string message)
    {
        var result = await CreateController().Send(new SendContactRequest(name, email, message));

        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Imię, email i wiadomość są wymagane.", bad.Value);
        await using var db = _db.NewContext();
        Assert.Empty(db.ContactMessages);
    }

    [Fact]
    public async Task List_ReturnsNewestMessagesFirst()
    {
        await using (var db = _db.NewContext())
        {
            db.ContactMessages.AddRange(
                new ContactMessage
                {
                    Id = Guid.NewGuid(), Name = "Stara", Email = "a@example.com", Message = "1",
                    CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new ContactMessage
                {
                    Id = Guid.NewGuid(), Name = "Nowa", Email = "b@example.com", Message = "2",
                    CreatedAt = new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc)
                });
            await db.SaveChangesAsync();
        }

        var result = await CreateController().List();

        var items = Assert.IsAssignableFrom<IReadOnlyList<ContactMessageDto>>(
            Assert.IsType<OkObjectResult>(result.Result).Value);
        Assert.Equal(["Nowa", "Stara"], items.Select(m => m.Name));
        Assert.Equal("b@example.com", items[0].Email);
    }

    [Fact]
    public async Task List_NoMessages_ReturnsEmptyList()
    {
        var result = await CreateController().List();

        var items = Assert.IsAssignableFrom<IReadOnlyList<ContactMessageDto>>(
            Assert.IsType<OkObjectResult>(result.Result).Value);
        Assert.Empty(items);
    }
}
