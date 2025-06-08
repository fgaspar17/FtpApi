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
    public async Task Delete(string userId, string filename, CancellationToken ct)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(filename))
                throw new CustomValidationException("Filename is empty.");

            var fileToDelete = await _context.FileMetadatas
                .SingleOrDefaultAsync(f => f.FileName.Trim().ToLower() == filename.Trim().ToLower());

            if (fileToDelete is null)
                throw new CustomValidationException("Filename not found.");

            await _ftpUtils.Delete(_ftpConfig.Value, userId, fileToDelete.FileName, ct);
            _logger.LogInformation("File {filename} deleted from the FTP server by {userId}.", filename, userId);

            fileToDelete.IsDeleted = true;
            _context.FileMetadatas.Update(fileToDelete);
            await _context.SaveChangesAsync(ct);
            _logger.LogInformation("File {filename} deleted from the database by {userId}.", filename, userId);
        }
        catch (CustomValidationException ex)
        {
            _logger.LogWarning(ex, "Error trying to validate the {filename} provided by {userId}.", filename, userId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error trying to delete file {filename} by {userId}.", filename, userId);
            throw;
        }
    }
}