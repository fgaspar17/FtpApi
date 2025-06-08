using FluentValidation.TestHelper;
using FtpApi.Application.DTOs;
using FtpApi.Application.Validators;

namespace FtpApi.Tests.ValidatorsTests;

public class UserLoginValidatorTests
{
    private readonly UserLoginValidator _validator = new();

    [Theory]
    [InlineData("JuanVI", "ValidTestP@ssword123")]
    [InlineData("JhonDoe", "Valid123!")]
    public void Validate_ShouldPass_WhenInputIsValid(string userName, string password)
    {
        // Arrange
        var loginDto = new UserLoginDto
        {
            UserName = userName,
            Password = password
        };

        // Act
        var result = _validator.TestValidate(loginDto);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenUserNameIsEmpty()
    {
        var dto = new UserLoginDto { UserName = "", Password = "Valid123!" };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(u => u.UserName);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPasswordIsEmpty()
    {
        // Arrange
        var dto = new UserLoginDto { UserName = "user", Password = "" };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(u => u.Password);
    }

    [Theory]
    [InlineData("sh0rtA!")]    // too short (7)
    [InlineData("abcdefgh")]    // no uppercase, number, or special
    [InlineData("ABCDEFGH")]    // no lowercase, number, or special
    [InlineData("abcDEFGH")]    // no number or special
    [InlineData("abcDEF12")]    // no special
    [InlineData("abc!@#DEF")]   // no number
    public void Validate_ShouldHaveError_WhenPasswordMissingRequiredComplexity(string password)
    {
        // Arrange
        var dto = new UserLoginDto { UserName = "user", Password = password };

        // Act
        var result = _validator.TestValidate(dto);

        // Assert
        result.ShouldHaveValidationErrorFor(u => u.Password);
    }
}