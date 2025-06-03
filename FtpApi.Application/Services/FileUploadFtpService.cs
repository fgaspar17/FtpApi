using FluentValidation;
using FtpApi.Application.DTOs;
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
        _validator.ValidateAndThrow(fileDto);

        var hashUtils = new HashUtils();
        var hash = await hashUtils.ComputeFileHash(fileDto.File!);
        var hashString = Convert.ToBase64String(hash);

        var fileData = FileUploadMapper.Map(fileDto, userId, hashString);

        await _ftpUtils.Upload(_ftpConfig.Value, userId, fileDto.File, ct);
        _context.FileMetadatas.Add(fileData);
        await _context.SaveChangesAsync();
    }
}