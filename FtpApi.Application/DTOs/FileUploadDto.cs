using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FtpApi.Application.DTOs;

public class FileUploadDto
{
    public IFormFile? File { get; set; }
    public string? Description { get; set; }
}
