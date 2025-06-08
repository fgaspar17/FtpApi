using FtpApi.Data;
using FtpApi.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FtpApi.Application.Services;

public class FileGetFilesFtpService : IFileGetFilesService
{
    private readonly AppDbContext _context;
    private readonly ILogger<FileGetFilesFtpService> _logger;

    public FileGetFilesFtpService(
        AppDbContext context,
        ILogger<FileGetFilesFtpService> logger)
    {
        _context = context;
        _logger = logger;
    }
    public async Task<List<FileMetadata>> GetAll(string userId, CancellationToken ct)
    {
        try
        {
            var result = await _context.FileMetadatas.AsNoTracking().Where(fm => fm.UserId == userId).ToListAsync(ct);
            _logger.LogInformation("Files retrieve from the database by {userId}.", userId);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error trying to retrieve FileMetadatas by {userId}.", userId);
            throw;
        }
    }
}
