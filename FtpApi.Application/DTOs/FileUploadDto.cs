using Microsoft.AspNetCore.Http;

namespace FtpApi.Application.DTOs;

public class FileUploadDto
{
    public IFormFile? File { get; set; }
    public string? Description { get; set; }
}