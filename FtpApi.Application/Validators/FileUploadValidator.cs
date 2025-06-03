using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FtpApi.Application.DTOs;
using FtpApi.Data.Enums;

namespace FtpApi.Application.Validators;

public class FileUploadValidator : AbstractValidator<FileUploadDto>
{
    public FileUploadValidator()
    {
        RuleFor(f => f.File)
            .NotNull().WithMessage("File is required.");

        RuleFor(f => f.File)
            .Must(file => file!.Length > 0).WithMessage("File cannot be empty.")
            .Must(file => IsExtensionAllowed(Path.GetExtension(file!.FileName))).WithMessage("Unsupported file extension.")
            .Must(file => IsContentTypeAllowed(file!.ContentType)).WithMessage("Unsupported content type.")
            // 5 MB Limit
            .Must(file => file!.Length <= 5 * 1024 * 1024).WithMessage("File must be less than 5MB.")
            .When(f => f.File != null);
    }

    private bool IsExtensionAllowed(string extension)
    {
        return Enum.TryParse<FileType>(extension.TrimStart('.'), ignoreCase: true, out _);
    }

    private bool IsContentTypeAllowed(string contentType)
    {
        return Constants.FileMime._contentTypesAllowed.Contains(contentType);
    }
}
