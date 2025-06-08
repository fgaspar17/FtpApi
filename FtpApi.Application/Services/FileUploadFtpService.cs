using FluentValidation;
using FtpApi.Application.Constants;
using FtpApi.Application.DTOs;
using FtpApi.Application.Exceptions;
using FtpApi.Application.Extensions;
using FtpApi.Application.Mappers;
using FtpApi.Application.Utils;
using FtpApi.Data;
using FtpApi.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FtpApi.Application.Services;

public class FileUploadFtpService : IFileUploadService
{
    private readonly IValidator<FileUploadDto> _validator;
    private readonly AppDbContext _context;
    private readonly UserManager<ApiUser> _userManager;
    private readonly ILogger<FileUploadFtpService> _logger;
    private readonly FtpUtils _ftpUtils;
    private readonly IOptions<Config.FtpConfig> _ftpConfig;

    public FileUploadFtpService(
        IValidator<FileUploadDto> validator,
        AppDbContext context,
        UserManager<ApiUser> userManager,
        ILogger<FileUploadFtpService> logger,
        FtpUtils ftpUtils,
        IOptions<Config.FtpConfig> ftpConfig)
    {
        _validator = validator;
        _context = context;
        _userManager = userManager;
        _logger = logger;
        _ftpUtils = ftpUtils;
        _ftpConfig = ftpConfig;
    }

    public async Task Upload(FileUploadDto fileDto, string userId, CancellationToken ct)
    {
        try
        {
            await _validator.ValidateAndThrowCustomAsync(fileDto);

            var hashUtils = new HashUtils();
            var hash = await hashUtils.ComputeFileHash(fileDto.File!);
            var hashString = Convert.ToBase64String(hash);

            var fileData = FileUploadMapper.Map(fileDto, userId, hashString);

            await _ftpUtils.Upload(_ftpConfig.Value, userId, fileDto.File, ct);
            _logger.LogInformation("File {filename} uploaded to the FTP server by {userId}.", fileDto.File.FileName, userId);

            _context.FileMetadatas.Add(fileData);
            await _context.SaveChangesAsync(ct);
            _logger.LogInformation("File {filename} uploaded to the database by {userId}.", fileDto.File.FileName, userId);
        }
        catch (CustomValidationException ex)
        {
            _logger.LogWarning(ex, "Error trying to validate the {@fileDto} provided by {userId}.", fileDto, userId);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error trying to upload file {filename} by {userId}.", fileDto.File?.FileName, userId);
            throw;
        }
    }
}