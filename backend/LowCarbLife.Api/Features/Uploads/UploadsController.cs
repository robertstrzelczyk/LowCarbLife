using LowCarbLife.Api.Features.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LowCarbLife.Api.Features.Uploads;

[ApiController]
[Route("api/uploads")]
[Authorize(Roles = Roles.Admin)]
public class UploadsController : ControllerBase
{
    private const long MaxBytes = 5 * 1024 * 1024;

    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/gif"
    };

    private static readonly Dictionary<string, string> Extensions = new(StringComparer.OrdinalIgnoreCase)
    {
        [".jpg"] = ".jpg",
        [".jpeg"] = ".jpg",
        [".png"] = ".png",
        [".webp"] = ".webp",
        [".gif"] = ".gif"
    };

    [HttpPost("images")]
    [RequestSizeLimit(MaxBytes)]
    public async Task<ActionResult<UploadImageResponse>> UploadImage(IFormFile? file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest("Wybierz zdjęcie.");
        }

        if (file.Length > MaxBytes)
        {
            return BadRequest("Zdjęcie może mieć maksymalnie 5 MB.");
        }

        var extension = Path.GetExtension(file.FileName);
        if (!Extensions.TryGetValue(extension, out var storedExtension))
        {
            return BadRequest("Dozwolone formaty to JPG, PNG, WEBP i GIF.");
        }

        var contentType = file.ContentType ?? string.Empty;
        if (contentType.Length > 0
            && !contentType.Equals("application/octet-stream", StringComparison.OrdinalIgnoreCase)
            && !AllowedContentTypes.Contains(contentType))
        {
            return BadRequest("Dozwolone formaty to JPG, PNG, WEBP i GIF.");
        }

        var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        Directory.CreateDirectory(uploads);

        var fileName = $"{Guid.NewGuid():N}{storedExtension}";
        var path = Path.Combine(uploads, fileName);
        await using (var stream = System.IO.File.Create(path))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        return Ok(new UploadImageResponse($"/uploads/{fileName}"));
    }
}

public record UploadImageResponse(string Url);
