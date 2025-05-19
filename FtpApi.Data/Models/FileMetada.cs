using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FtpApi.Data.Enums;

namespace FtpApi.Data.Models;

[Table("FileMetadatas")]
public class FileMetadata
{
    [Key]
    public int Id { get; set; }

    [Required, MaxLength(255)]
    public required string FileName { get; set; }
    [Required]
    public FileType FileType { get; set; }
    [Required, Range(0, long.MaxValue)]
    public long FileSizeBytes { get; set; }
    [Required]
    public required string FilePath { get; set; }

    public bool IsCompressed { get; set; }
    public bool IsEncrypted { get; set; }
    public bool IsDeleted { get; set; }

    [Required]
    public required string UserId { get; set; }
    public ApiUser? ApiUser { get; set; }
    [Required]
    public DateTime UploadedAtUtc { get; set; }

    public string? SHA256Hash { get; set; }
    [MaxLength(1_000)]
    public string? Description { get; set; }
}
