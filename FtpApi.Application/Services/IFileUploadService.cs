using FtpApi.Application.DTOs;

namespace FtpApi.Application.Services;

public interface IFileUploadService
{
    public Task Upload(FileUploadDto fileDto, string userId, CancellationToken ct);
}