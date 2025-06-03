using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentFTP;
using FtpApi.Application.Constants;
using FtpApi.Application.Utils;
using FtpApi.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FtpApi.Application.Services;

public class FileDownloadFtpService : IFileDownloadService
{
    private readonly AppDbContext _context;
    private readonly FtpUtils _ftpUtils;
    private readonly IOptions<Config.FtpConfig> _ftpConfig;

    public FileDownloadFtpService(
        AppDbContext context,
        FtpUtils ftpUtils,
        IOptions<Config.FtpConfig> ftpConfig)
    {
        _context = context;
        _ftpUtils = ftpUtils;
        _ftpConfig = ftpConfig;
    }
    public async Task<Stream> Download(string userId, string filename, CancellationToken ct)
    {
        if (!await _context.FileMetadatas
            .AnyAsync(f => f.FileName.Trim().ToLower() == filename.Trim().ToLower()))
            throw new InvalidOperationException();

        return await _ftpUtils.Download(_ftpConfig.Value, userId, filename, ct);
    }
}