namespace LowCarbLife.Api.Features.Blog;

public record BlogPostListItemDto(
    Guid Id,
    string Title,
    string Excerpt,
    string? ImageUrl,
    DateTime CreatedAt,
    string? AuthorName);

public record BlogPostDetailDto(
    Guid Id,
    string Title,
    string Content,
    string? ImageUrl,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    string? AuthorName);

public record UpsertBlogPostRequest(string Title, string Content, string? ImageUrl);
