using FtpApi.Data;
using FtpApi.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FtpApi.Application.Services;

public class FileGetFilesFtpService : IFileGetFilesService
{
    private readonly AppDbContext _context;

    public FileGetFilesFtpService(
        AppDbContext context)
    {
        _context = context;
    }
    public async Task<List<FileMetadata>> GetAll(string userId, CancellationToken ct)
    {
        return await _context.FileMetadatas.AsNoTracking().Where(fm => fm.UserId == userId).ToListAsync();
    }
}
