using FtpApi.Application.DTOs;
using FtpApi.Data.Enums;
using FtpApi.Data.Models;

namespace FtpApi.Application.Mappers;

internal class FileUploadMapper
{
    public static FileMetadata Map(FileUploadDto fileDto, string userId, string hashComputed)
    {
        var file = fileDto.File;
        var fileName = Path.GetFileName(file.FileName);
        var extension = Path.GetExtension(fileName).TrimStart('.');

        return new FileMetadata
        {
            FileName = fileName,
            FileType = Enum.TryParse<FileType>(extension, ignoreCase: true, out var fileType)
                ? fileType
                : throw new InvalidOperationException("Unsupported file type."),
            FileSizeBytes = file.Length,
            FilePath = Path.Combine("files", userId, file.FileName),
            // TODO: add compression
            IsCompressed = false,
            // TODO: add compression
            IsEncrypted = false,
            IsDeleted = false,
            UserId = userId,
            UploadedAtUtc = DateTime.UtcNow,
            SHA256Hash = hashComputed,
            Description = fileDto.Description
        };
    }
}