using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FtpApi.Data.Models;

namespace FtpApi.Application.Services;

public interface IFileGetFilesService
{
    public Task<List<FileMetadata>> GetAll(string userId, CancellationToken ct);
}