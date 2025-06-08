using FtpApi.Application.Constants;
using FtpApi.Application.Exceptions;
using FtpApi.Application.Utils;
using FtpApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FtpApi.Application.Services;

public class FileDownloadFtpService : IFileDownloadService
{
    private readonly AppDbContext _context;
    private readonly FtpUtils _ftpUtils;
    private readonly IOptions<Config.FtpConfig> _ftpConfig;
    private readonly ILogger<FileDownloadFtpService> _logger;

    public FileDownloadFtpService(
        AppDbContext context,
        FtpUtils ftpUtils,
        IOptions<Config.FtpConfig> ftpConfig,
        ILogger<FileDownloadFtpService> logger)
    {
        _context = context;
        _ftpUtils = ftpUtils;
        _ftpConfig = ftpConfig;
        _logger = logger;
    }
    public async Task<Stream> Download(string userId, string filename, CancellationToken ct)
    {
        try
        {
            if (!await _context.FileMetadatas
            .AnyAsync(f => f.FileName.Trim().ToLower() == filename.Trim().ToLower(), ct))
                throw new CustomValidationException("Filename not found.");
            _logger.LogInformation("File {filename} retrieved from the database by {userId}.", filename, userId);

            var result = await _ftpUtils.Download(_ftpConfig.Value, userId, filename, ct);
            _logger.LogInformation("File {filename} downloaded from the FTP server by {userId}.", filename, userId);

            return result;
        }
        catch (CustomValidationException ex)
        {
            _logger.LogWarning(ex, "Error trying to validate the {@filename} provided by {userId}.", filename, userId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error trying to download file {filename} by {userId}.", filename, userId);
            throw;
        }
        
    }
}