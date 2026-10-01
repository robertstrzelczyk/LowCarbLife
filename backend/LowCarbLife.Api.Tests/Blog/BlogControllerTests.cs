using System.Security.Claims;
using LowCarbLife.Api.Features.Auth;
using LowCarbLife.Api.Features.Blog;
using LowCarbLife.Api.Tests.TestSupport;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace LowCarbLife.Api.Tests.Blog;

public class BlogControllerTests
{
    private readonly TestDb _db = new();
    private readonly UserManager<ApplicationUser> _userManager = TestFactories.UserManager();

    private BlogController CreateController() =>
        new BlogController(_db.NewContext(), _userManager).WithHttpContext();

    private async Task<ApplicationUser> SeedUserAsync(
        string id = "author-1",
        string email = "autor@example.com",
        string? displayName = "Autor")
    {
        await using var db = _db.NewContext();
        var user = TestFactories.User(id, email, displayName);
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return user;
    }

    private async Task<BlogPost> SeedPostAsync(
        string title = "Tytuł",
        string content = "Treść",
        string authorId = "author-1",
        DateTime? createdAt = null)
    {
        await using var db = _db.NewContext();
        var post = new BlogPost
        {
            Id = Guid.NewGuid(),
            Title = title,
            Content = content,
            AuthorId = authorId,
            CreatedAt = createdAt ?? DateTime.UtcNow
        };
        db.BlogPosts.Add(post);
        await db.SaveChangesAsync();
        return post;
    }

    // ---------- List ----------

    [Fact]
    public async Task List_ReturnsNewestPostsFirst()
    {
        await SeedUserAsync();
        await SeedPostAsync("Stary", createdAt: new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));
        await SeedPostAsync("Nowy", createdAt: new DateTime(2026, 6, 1, 0, 0, 0, DateTimeKind.Utc));
        await SeedPostAsync("Średni", createdAt: new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc));

        var result = await CreateController().List();

        var items = Assert.IsAssignableFrom<IReadOnlyList<BlogPostListItemDto>>(
            Assert.IsType<OkObjectResult>(result.Result).Value);
        Assert.Equal(["Nowy", "Średni", "Stary"], items.Select(i => i.Title));
    }

    [Fact]
    public async Task List_ShortContent_IsReturnedAsIsInExcerpt()
    {
        await SeedUserAsync();
        await SeedPostAsync(content: "  Krótka treść  ");

        var result = await CreateController().List();

        var item = Assert.Single(Assert.IsAssignableFrom<IReadOnlyList<BlogPostListItemDto>>(
            Assert.IsType<OkObjectResult>(result.Result).Value));
        Assert.Equal("Krótka treść", item.Excerpt);
    }

    [Fact]
    public async Task List_ContentExactlyAtLimit_IsNotTruncated()
    {
        await SeedUserAsync();
        var content = new string('a', 180);
        await SeedPostAsync(content: content);

        var result = await CreateController().List();

        var item = Assert.Single(Assert.IsAssignableFrom<IReadOnlyList<BlogPostListItemDto>>(
            Assert.IsType<OkObjectResult>(result.Result).Value));
        Assert.Equal(content, item.Excerpt);
    }

    [Fact]
    public async Task List_LongContent_IsTruncatedWithEllipsis()
    {
        await SeedUserAsync();
        await SeedPostAsync(content: new string('a', 181));

        var result = await CreateController().List();

        var item = Assert.Single(Assert.IsAssignableFrom<IReadOnlyList<BlogPostListItemDto>>(
            Assert.IsType<OkObjectResult>(result.Result).Value));
        Assert.Equal(new string('a', 180) + "…", item.Excerpt);
    }

    [Fact]
    public async Task List_AuthorName_PrefersDisplayNameThenEmail()
    {
        await SeedUserAsync("with-name", "a@example.com", "Ania");
        await SeedUserAsync("no-name", "b@example.com", null);
        await SeedPostAsync("Z nazwą", authorId: "with-name", createdAt: new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc));
        await SeedPostAsync("Bez nazwy", authorId: "no-name", createdAt: new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc));

        var result = await CreateController().List();

        var items = Assert.IsAssignableFrom<IReadOnlyList<BlogPostListItemDto>>(
            Assert.IsType<OkObjectResult>(result.Result).Value);
        Assert.Equal("Ania", items[0].AuthorName);
        Assert.Equal("b@example.com", items[1].AuthorName);
    }

    [Fact]
    public async Task List_UnknownAuthor_HasNullAuthorName()
    {
        await SeedPostAsync(authorId: "ghost");

        var result = await CreateController().List();

        var item = Assert.Single(Assert.IsAssignableFrom<IReadOnlyList<BlogPostListItemDto>>(
            Assert.IsType<OkObjectResult>(result.Result).Value));
        Assert.Null(item.AuthorName);
    }

    // ---------- Get ----------

    [Fact]
    public async Task Get_UnknownId_ReturnsNotFound()
    {
        var result = await CreateController().Get(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Get_ExistingPost_ReturnsFullContentAndAuthor()
    {
        await SeedUserAsync(displayName: "Autor");
        var post = await SeedPostAsync("Wpis", "Pełna treść wpisu");

        var result = await CreateController().Get(post.Id);

        var detail = Assert.IsType<BlogPostDetailDto>(Assert.IsType<OkObjectResult>(result.Result).Value);
        Assert.Equal("Wpis", detail.Title);
        Assert.Equal("Pełna treść wpisu", detail.Content);
        Assert.Equal("Autor", detail.AuthorName);
        Assert.Null(detail.UpdatedAt);
    }

    // ---------- Create ----------

    [Fact]
    public async Task Create_NoCurrentUser_ReturnsUnauthorized()
    {
        _userManager.GetUserAsync(Arg.Any<ClaimsPrincipal>()).Returns((ApplicationUser?)null);

        var result = await CreateController().Create(new UpsertBlogPostRequest("Tytuł", "Treść", null));

        Assert.IsType<UnauthorizedResult>(result.Result);
        await using var db = _db.NewContext();
        Assert.Empty(db.BlogPosts);
    }

    [Theory]
    [InlineData("", "treść")]
    [InlineData("   ", "treść")]
    [InlineData("tytuł", "")]
    [InlineData("tytuł", "   ")]
    public async Task Create_MissingTitleOrContent_ReturnsBadRequest(string title, string content)
    {
        _userManager.GetUserAsync(Arg.Any<ClaimsPrincipal>()).Returns(TestFactories.User());

        var result = await CreateController().Create(new UpsertBlogPostRequest(title, content, null));

        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Tytuł i treść są wymagane.", bad.Value);
        await using var db = _db.NewContext();
        Assert.Empty(db.BlogPosts);
    }

    [Fact]
    public async Task Create_ValidRequest_StoresTrimmedPostWithAuthor()
    {
        var user = TestFactories.User("admin-1", "admin@example.com", "Admin");
        _userManager.GetUserAsync(Arg.Any<ClaimsPrincipal>()).Returns(user);
        var before = DateTime.UtcNow;

        var result = await CreateController().Create(
            new UpsertBlogPostRequest("  Nowy wpis  ", "  Treść wpisu  ", "  https://example.com/a.jpg  "));

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        Assert.Equal(nameof(BlogController.Get), created.ActionName);
        var detail = Assert.IsType<BlogPostDetailDto>(created.Value);
        Assert.Equal("Nowy wpis", detail.Title);
        Assert.Equal("Admin", detail.AuthorName);

        await using var db = _db.NewContext();
        var stored = await db.BlogPosts.SingleAsync();
        Assert.Equal(detail.Id, stored.Id);
        Assert.Equal("Nowy wpis", stored.Title);
        Assert.Equal("Treść wpisu", stored.Content);
        Assert.Equal("https://example.com/a.jpg", stored.ImageUrl);
        Assert.Equal("admin-1", stored.AuthorId);
        Assert.True(stored.CreatedAt >= before);
        Assert.Null(stored.UpdatedAt);
    }

    [Fact]
    public async Task Create_BlankImageUrl_StoresNull()
    {
        _userManager.GetUserAsync(Arg.Any<ClaimsPrincipal>()).Returns(TestFactories.User());

        await CreateController().Create(new UpsertBlogPostRequest("Tytuł", "Treść", "  "));

        await using var db = _db.NewContext();
        Assert.Null((await db.BlogPosts.SingleAsync()).ImageUrl);
    }

    [Fact]
    public async Task Create_UserWithoutDisplayName_UsesEmailAsAuthorName()
    {
        _userManager.GetUserAsync(Arg.Any<ClaimsPrincipal>())
            .Returns(TestFactories.User(email: "jan@example.com", displayName: null));

        var result = await CreateController().Create(new UpsertBlogPostRequest("Tytuł", "Treść", null));

        var detail = Assert.IsType<BlogPostDetailDto>(
            Assert.IsType<CreatedAtActionResult>(result.Result).Value);
        Assert.Equal("jan@example.com", detail.AuthorName);
    }

    // ---------- Update ----------

    [Fact]
    public async Task Update_UnknownId_ReturnsNotFound()
    {
        var result = await CreateController().Update(
            Guid.NewGuid(), new UpsertBlogPostRequest("Tytuł", "Treść", null));

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Update_BlankTitleOrContent_ReturnsBadRequestAndKeepsPost()
    {
        var post = await SeedPostAsync("Oryginał", "Treść");

        var result = await CreateController().Update(post.Id, new UpsertBlogPostRequest("  ", "Nowa", null));

        Assert.IsType<BadRequestObjectResult>(result.Result);
        await using var db = _db.NewContext();
        Assert.Equal("Oryginał", (await db.BlogPosts.SingleAsync()).Title);
    }

    [Fact]
    public async Task Update_ValidRequest_UpdatesFieldsAndSetsUpdatedAt()
    {
        await SeedUserAsync();
        var post = await SeedPostAsync("Stary", "Stara treść");

        var result = await CreateController().Update(
            post.Id, new UpsertBlogPostRequest(" Nowy ", " Nowa treść ", " /uploads/a.png "));

        var detail = Assert.IsType<BlogPostDetailDto>(Assert.IsType<OkObjectResult>(result.Result).Value);
        Assert.Equal("Nowy", detail.Title);
        Assert.Equal("Autor", detail.AuthorName);
        Assert.NotNull(detail.UpdatedAt);

        await using var db = _db.NewContext();
        var stored = await db.BlogPosts.SingleAsync();
        Assert.Equal("Nowy", stored.Title);
        Assert.Equal("Nowa treść", stored.Content);
        Assert.Equal("/uploads/a.png", stored.ImageUrl);
        Assert.NotNull(stored.UpdatedAt);
    }

    // ---------- Delete ----------

    [Fact]
    public async Task Delete_UnknownId_ReturnsNotFound()
    {
        var result = await CreateController().Delete(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_ExistingPost_RemovesOnlyThatPost()
    {
        var keep = await SeedPostAsync("Zostaje");
        var remove = await SeedPostAsync("Do usunięcia");

        var result = await CreateController().Delete(remove.Id);

        Assert.IsType<NoContentResult>(result);
        await using var db = _db.NewContext();
        Assert.Equal(keep.Id, (await db.BlogPosts.SingleAsync()).Id);
    }
}
