using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FtpApi.Application.Utils;

internal class HashUtils
{
    public async Task<byte[]> ComputeFileHash(IFormFile file)
    {
        var stream = file.OpenReadStream();
        return await System.Security.Cryptography.SHA256.HashDataAsync(stream);
    }
}
