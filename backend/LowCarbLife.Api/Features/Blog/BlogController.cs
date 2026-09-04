using LowCarbLife.Api.Data;
using LowCarbLife.Api.Features.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LowCarbLife.Api.Features.Blog;

[ApiController]
[Route("api/blog")]
public class BlogController(AppDbContext db, UserManager<ApplicationUser> userManager) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IReadOnlyList<BlogPostListItemDto>>> List()
    {
        var posts = await db.BlogPosts
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        var authorIds = posts.Select(p => p.AuthorId).Distinct().ToList();
        var authors = await db.Users
            .AsNoTracking()
            .Where(u => authorIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.DisplayName ?? u.Email);

        return Ok(posts.Select(p => new BlogPostListItemDto(
            p.Id,
            p.Title,
            Excerpt(p.Content),
            p.ImageUrl,
            p.CreatedAt,
            authors.GetValueOrDefault(p.AuthorId))).ToList());
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<BlogPostDetailDto>> Get(Guid id)
    {
        var post = await db.BlogPosts.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (post is null)
        {
            return NotFound();
        }

        var author = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == post.AuthorId);
        return Ok(ToDetail(post, author?.DisplayName ?? author?.Email));
    }

    [HttpPost]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<BlogPostDetailDto>> Create(UpsertBlogPostRequest request)
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Content))
        {
            return BadRequest("Tytuł i treść są wymagane.");
        }

        var post = new BlogPost
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            Content = request.Content.Trim(),
            ImageUrl = NullIfEmpty(request.ImageUrl),
            CreatedAt = DateTime.UtcNow,
            AuthorId = user.Id
        };

        db.BlogPosts.Add(post);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = post.Id }, ToDetail(post, user.DisplayName ?? user.Email));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<ActionResult<BlogPostDetailDto>> Update(Guid id, UpsertBlogPostRequest request)
    {
        var post = await db.BlogPosts.FirstOrDefaultAsync(p => p.Id == id);
        if (post is null)
        {
            return NotFound();
        }

        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Content))
        {
            return BadRequest("Tytuł i treść są wymagane.");
        }

        post.Title = request.Title.Trim();
        post.Content = request.Content.Trim();
        post.ImageUrl = NullIfEmpty(request.ImageUrl);
        post.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        var author = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == post.AuthorId);
        return Ok(ToDetail(post, author?.DisplayName ?? author?.Email));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var post = await db.BlogPosts.FirstOrDefaultAsync(p => p.Id == id);
        if (post is null)
        {
            return NotFound();
        }

        db.BlogPosts.Remove(post);
        await db.SaveChangesAsync();
        return NoContent();
    }

    private static BlogPostDetailDto ToDetail(BlogPost post, string? authorName) =>
        new(post.Id, post.Title, post.Content, post.ImageUrl, post.CreatedAt, post.UpdatedAt, authorName);

    private static string Excerpt(string content)
    {
        var normalized = content.Replace("\r\n", "\n").Trim();
        return normalized.Length <= 180 ? normalized : normalized[..180] + "…";
    }

    private static string? NullIfEmpty(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
