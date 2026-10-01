using LowCarbLife.Api.Features.Uploads;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LowCarbLife.Api.Tests.Uploads;

public class UploadsControllerTests
{
    private const string FormatError = "Dozwolone formaty to JPG, PNG, WEBP i GIF.";

    private static FormFile CreateFile(
        string fileName,
        string contentType,
        long? declaredLength = null,
        byte[]? content = null)
    {
        var bytes = content ?? [1, 2, 3, 4];
        return new FormFile(new MemoryStream(bytes), 0, declaredLength ?? bytes.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }

    private static string UploadsDirectory =>
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

    [Fact]
    public async Task UploadImage_NoFile_ReturnsBadRequest()
    {
        var result = await new UploadsController().UploadImage(null, CancellationToken.None);

        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Wybierz zdjęcie.", bad.Value);
    }

    [Fact]
    public async Task UploadImage_EmptyFile_ReturnsBadRequest()
    {
        var file = CreateFile("a.png", "image/png", content: []);

        var result = await new UploadsController().UploadImage(file, CancellationToken.None);

        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Wybierz zdjęcie.", bad.Value);
    }

    [Fact]
    public async Task UploadImage_FileLargerThanFiveMegabytes_ReturnsBadRequest()
    {
        var file = CreateFile("duze.png", "image/png", declaredLength: 5 * 1024 * 1024 + 1);

        var result = await new UploadsController().UploadImage(file, CancellationToken.None);

        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal("Zdjęcie może mieć maksymalnie 5 MB.", bad.Value);
    }

    [Theory]
    [InlineData("skrypt.exe")]
    [InlineData("dokument.pdf")]
    [InlineData("bez-rozszerzenia")]
    [InlineData("obraz.svg")]
    public async Task UploadImage_DisallowedExtension_ReturnsBadRequest(string fileName)
    {
        var file = CreateFile(fileName, "image/png");

        var result = await new UploadsController().UploadImage(file, CancellationToken.None);

        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(FormatError, bad.Value);
    }

    [Theory]
    [InlineData("text/html")]
    [InlineData("application/pdf")]
    [InlineData("image/svg+xml")]
    public async Task UploadImage_DisallowedContentType_ReturnsBadRequest(string contentType)
    {
        var file = CreateFile("zdjecie.png", contentType);

        var result = await new UploadsController().UploadImage(file, CancellationToken.None);

        var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(FormatError, bad.Value);
    }

    [Theory]
    [InlineData("zdjecie.png", "image/png", ".png")]
    [InlineData("ZDJECIE.PNG", "image/png", ".png")]
    [InlineData("zdjecie.jpg", "image/jpeg", ".jpg")]
    [InlineData("zdjecie.jpeg", "image/jpeg", ".jpg")]
    [InlineData("zdjecie.webp", "image/webp", ".webp")]
    [InlineData("zdjecie.gif", "image/gif", ".gif")]
    [InlineData("zdjecie.png", "application/octet-stream", ".png")]
    public async Task UploadImage_AllowedFile_SavesItAndReturnsPublicUrl(
        string fileName, string contentType, string expectedExtension)
    {
        var file = CreateFile(fileName, contentType, content: [10, 20, 30, 40, 50]);
        string? savedPath = null;

        try
        {
            var result = await new UploadsController().UploadImage(file, CancellationToken.None);

            var response = Assert.IsType<UploadImageResponse>(Assert.IsType<OkObjectResult>(result.Result).Value);
            Assert.StartsWith("/uploads/", response.Url);
            Assert.EndsWith(expectedExtension, response.Url);

            savedPath = Path.Combine(UploadsDirectory, Path.GetFileName(response.Url));
            Assert.True(File.Exists(savedPath));
            Assert.Equal(new byte[] { 10, 20, 30, 40, 50 }, await File.ReadAllBytesAsync(savedPath));
        }
        finally
        {
            if (savedPath is not null && File.Exists(savedPath))
            {
                File.Delete(savedPath);
            }
        }
    }

    [Fact]
    public async Task UploadImage_UsesRandomFileNameInsteadOfClientName()
    {
        var file = CreateFile("../../zlosliwa.png", "image/png");
        string? savedPath = null;

        try
        {
            var result = await new UploadsController().UploadImage(file, CancellationToken.None);

            var response = Assert.IsType<UploadImageResponse>(Assert.IsType<OkObjectResult>(result.Result).Value);
            Assert.DoesNotContain("zlosliwa", response.Url);
            Assert.DoesNotContain("..", response.Url);
            savedPath = Path.Combine(UploadsDirectory, Path.GetFileName(response.Url));
            Assert.True(Guid.TryParseExact(Path.GetFileNameWithoutExtension(response.Url), "N", out _));
        }
        finally
        {
            if (savedPath is not null && File.Exists(savedPath))
            {
                File.Delete(savedPath);
            }
        }
    }
}
