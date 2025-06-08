using System.Text;
using FluentValidation.TestHelper;
using FtpApi.Application.DTOs;
using FtpApi.Application.Validators;
using Microsoft.AspNetCore.Http;
using Moq;

namespace FtpApi.UnitTests.Validators;

public class FileUploadValidatorTests
{
    private readonly FileUploadValidator _validator;

    public FileUploadValidatorTests()
    {
        _validator = new FileUploadValidator();
    }

    private IFormFile CreateMockFile(string fileName, string contentType, long length)
    {
        var content = new MemoryStream(Encoding.UTF8.GetBytes("Dummy content."));

        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.ContentType).Returns(contentType);
        fileMock.Setup(f => f.Length).Returns(length);
        fileMock.Setup(f => f.OpenReadStream()).Returns(content);

        return fileMock.Object;
    }

    [Fact]
    public void Validate_ShouldPass_WhenFileIsValid()
    {
        var file = CreateMockFile("document.pdf", "application/pdf", 1024);
        var dto = new FileUploadDto { File = file };

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldFail_WhenFileIsNull()
    {
        var dto = new FileUploadDto { File = null! };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.File)
            .WithErrorMessage("File is required.");
    }

    [Fact]
    public void Validate_ShouldFail_WhenFileIsEmpty()
    {
        var file = CreateMockFile("document.pdf", "application/pdf", 0);
        var dto = new FileUploadDto { File = file };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.File)
            .WithErrorMessage("File cannot be empty.");
    }

    [Fact]
    public void Validate_ShouldFail_WhenExtensionNotAllowed()
    {
        var file = CreateMockFile("document.exe", "application/pdf", 1024);
        var dto = new FileUploadDto { File = file };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.File)
            .WithErrorMessage("Unsupported file extension.");
    }

    [Fact]
    public void Validate_ShouldFail_WhenContentTypeNotAllowed()
    {
        var file = CreateMockFile("document.pdf", "application/x-msdownload", 1024);
        var dto = new FileUploadDto { File = file };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.File)
            .WithErrorMessage("Unsupported content type.");
    }

    [Fact]
    public void Validate_ShouldFail_WhenFileTooLarge()
    {
        var file = CreateMockFile("document.pdf", "application/pdf", 6 * 1024 * 1024);
        var dto = new FileUploadDto { File = file };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.File)
            .WithErrorMessage("File must be less than 5MB.");
    }
}