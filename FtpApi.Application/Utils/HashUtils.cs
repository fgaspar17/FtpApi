using Microsoft.AspNetCore.Http;

namespace FtpApi.Application.Utils;

public class HashUtils
{
    public async Task<byte[]> ComputeFileHash(IFormFile file)
    {
        var stream = file.OpenReadStream();
        return await System.Security.Cryptography.SHA256.HashDataAsync(stream);
    }
}