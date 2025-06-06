using System.Text;
using Microsoft.AspNetCore.Http;

namespace FtpApi.IntegrationTests.Helpers;

public static class FormFileMockHelper
{
    public static IFormFile CreateFormFile(string content, string fileName, string contentType = "text/plain")
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        var stream = new MemoryStream(bytes);

        return new FormFile(stream, 0, bytes.Length, "file", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };
    }
}

