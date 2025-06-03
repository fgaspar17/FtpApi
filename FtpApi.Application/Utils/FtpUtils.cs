using System.Net;
using FluentFTP;
using Microsoft.AspNetCore.Http;

namespace FtpApi.Application.Utils;

public class FtpUtils
{

    public async Task Upload(Config.FtpConfig config, string userId, IFormFile file, CancellationToken ct)
    {
        using (var client = new AsyncFtpClient())
        {
            client.Host = config.Host;
            client.Credentials = new NetworkCredential(config.Username, config.Password);

            await client.Connect(ct);
            // TODO: Create folder per user
            await client.SetWorkingDirectory("/files", ct);
            await client.UploadStream(file.OpenReadStream(), Path.Combine(userId, file.FileName), FtpRemoteExists.Overwrite, createRemoteDir: true, token: ct);
        }
    }

    public async Task Delete(Config.FtpConfig config, string userId, string fileName, CancellationToken ct)
    {
        using (var client = new AsyncFtpClient())
        {
            client.Host = config.Host;
            client.Credentials = new NetworkCredential(config.Username, config.Password);

            await client.Connect(ct);
            // TODO: folder per user
            await client.SetWorkingDirectory("/files", ct);
            await client.DeleteFile(Path.Combine(userId, fileName), token: ct);
        }
    }

    public async Task<Stream> Download(Config.FtpConfig config, string userId, string fileName, CancellationToken ct)
    {
        using (var client = new AsyncFtpClient())
        {
            client.Host = config.Host;
            client.Credentials = new NetworkCredential(config.Username, config.Password);

            await client.Connect(ct);
            // TODO: folder per user
            await client.SetWorkingDirectory("/files", ct);
            MemoryStream stream = new MemoryStream();
            await client.DownloadStream(stream, Path.Combine(userId, fileName), token: ct);

            return stream;
        }
    }
}
