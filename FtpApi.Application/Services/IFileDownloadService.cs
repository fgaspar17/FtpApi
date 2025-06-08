namespace FtpApi.Application.Services;

public interface IFileDownloadService
{
    public Task<Stream> Download(string userId, string filename, CancellationToken ct);
}