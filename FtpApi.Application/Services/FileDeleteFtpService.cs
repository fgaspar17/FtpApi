using FtpApi.Application.Exceptions;
using FtpApi.Application.Utils;
using FtpApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FtpApi.Application.Services;

public class FileDeleteFtpService : IFileDeleteService
{
    private readonly AppDbContext _context;
    private readonly ILogger<FileDeleteFtpService> _logger;
    private readonly FtpUtils _ftpUtils;
    private readonly IOptions<Config.FtpConfig> _ftpConfig;

    public FileDeleteFtpService(
        AppDbContext context,
        ILogger<FileDeleteFtpService> logger,
        FtpUtils ftpUtils,
        IOptions<Config.FtpConfig> ftpConfig)
    {
        _context = context;
        _logger = logger;
        _ftpUtils = ftpUtils;
        _ftpConfig = ftpConfig;
    }
    public async Task Delete(string userId, string fileName, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new CustomValidationException("Filename is empty.");

        var files = _context.FileMetadatas.ToList();

        var fileToDelete = await _context.FileMetadatas
            .SingleOrDefaultAsync(f => f.FileName.Trim().ToLower() == fileName.Trim().ToLower());

        if (fileToDelete is null)
            throw new CustomValidationException("Filename not found.");

        fileToDelete.IsDeleted = true;

        await _ftpUtils.Delete(_ftpConfig.Value, userId, fileToDelete.FileName, ct);
        _context.FileMetadatas.Update(fileToDelete);
        await _context.SaveChangesAsync();
    }
}
