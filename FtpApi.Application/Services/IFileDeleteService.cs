namespace FtpApi.Application.Services;

public interface IFileDeleteService
{
    public Task Delete(string userId, string filename, CancellationToken ct);
}