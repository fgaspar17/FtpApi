using FtpApi.Application.DTOs;
using FtpApi.Application.Utils;
using FtpApi.Data;
using FtpApi.IntegrationTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FtpApi.IntegrationTests.ServicesTests;

public class FtpServicesTests : IClassFixture<FtpTestFixture>
{
    private readonly FtpTestFixture _fixture;

    public FtpServicesTests(FtpTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task UploadService_ShouldSuccess_WhenFileIsValid()
    {
        // Arrange
        var fileDto = new FileUploadDto
        {
            Description = "test",
            File = FormFileMockHelper.CreateFormFile("test content", "test.pdf")
        };

        var _dbContext = _fixture.ServiceProvider.GetRequiredService<AppDbContext>();

        CancellationTokenSource cts = new CancellationTokenSource();
        var ct = cts.Token;

        // Act
        await _fixture.UploadService.Upload(fileDto, _fixture.UserId, ct);


        // Assert
        var metadata = await _dbContext.FileMetadatas.FirstOrDefaultAsync();
        Assert.NotNull(metadata);
        Assert.Equal("test.pdf", metadata.FileName);
        Assert.Equal("test", metadata.Description);
        Assert.Equal(_fixture.UserId, metadata.UserId);
        Assert.False(string.IsNullOrWhiteSpace(metadata.SHA256Hash));
    }

    [Fact]
    public async Task DeleteService_ShouldSuccess_WhenFileExists()
    {
        // Arrange
        var fileDto = new FileUploadDto
        {
            Description = "test",
            File = FormFileMockHelper.CreateFormFile("test content", "test.pdf")
        };

        var dbContext = _fixture.ServiceProvider.GetRequiredService<AppDbContext>();
        var uploadService = _fixture.UploadService;
        var deleteService = _fixture.DeleteService;

        var ct = CancellationToken.None;

        // Upload file first
        await uploadService.Upload(fileDto, _fixture.UserId, ct);

        // Act
        await deleteService.Delete(_fixture.UserId, "test.pdf", ct);

        // Assert
        var metadata = await dbContext.FileMetadatas.IgnoreQueryFilters().FirstOrDefaultAsync();
        Assert.NotNull(metadata);
        Assert.True(metadata.IsDeleted);
    }

    [Fact]
    public async Task DownloadService_ShouldReturnCorrectFileStream_WhenFileExists()
    {
        // Arrange
        var fileContent = "test content";
        var fileDto = new FileUploadDto
        {
            Description = "test",
            File = FormFileMockHelper.CreateFormFile(fileContent, "test.pdf")
        };

        var dbContext = _fixture.ServiceProvider.GetRequiredService<AppDbContext>();
        var uploadService = _fixture.UploadService;
        var downloadService = _fixture.DownloadService;

        var ct = CancellationToken.None;

        // Upload the file first
        await uploadService.Upload(fileDto, _fixture.UserId, ct);

        // Act
        var stream = await downloadService.Download(_fixture.UserId, "test.pdf", ct);
        stream.Position = 0;

        // Assert
        using var reader = new StreamReader(stream);
        var downloadedContent = await reader.ReadToEndAsync();

        Assert.Equal(fileContent, downloadedContent);

        // Optionally check hash match with DB
        var fileMetadata = await dbContext.FileMetadatas.FirstOrDefaultAsync();
        Assert.NotNull(fileMetadata);

        var originalHash = fileMetadata.SHA256Hash;
        stream.Position = 0;

        var hashUtils = new HashUtils();
        var computedHash = await hashUtils.ComputeFileHash(fileDto.File);
        var computedHashBase64 = Convert.ToBase64String(computedHash);

        Assert.Equal(originalHash, computedHashBase64);
    }
}