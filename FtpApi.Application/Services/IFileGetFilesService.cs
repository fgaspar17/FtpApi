using FtpApi.Data.Models;

namespace FtpApi.Application.Services;

public interface IFileGetFilesService
{
    public Task<List<FileMetadata>> GetAll(string userId, CancellationToken ct);
}