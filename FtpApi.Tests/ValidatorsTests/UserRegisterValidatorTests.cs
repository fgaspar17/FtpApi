using FluentValidation.TestHelper;
using FtpApi.Application.DTOs;
using FtpApi.Application.Validators;

namespace FtpApi.Tests.ValidatorsTests;

public class UserRegisterValidatorTests
{
    private readonly UserRegisterValidator _validator = new();

    [Fact]
    public void Validate_ShouldPass_WhenInputIsValid()
    {
        var dto = new UserRegisterDto
        {
            UserName = "validuser",
            Password = "Valid123!",
            ConfirmedPassword = "Valid123!"
        };

        var result = _validator.TestValidate(dto);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenUserNameIs_Empty()
    {
        var dto = new UserRegisterDto
        {
            UserName = "",
            Password = "Valid123!",
            ConfirmedPassword = "Valid123!"
        };

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.UserName);
    }

    [Fact]
    public void Validate_ShouldHaveError_WhenPasswordTooShort()
    {
        var dto = new UserRegisterDto
        {
            UserName = "user",
            Password = "Va1!",
            ConfirmedPassword = "Va1!"
        };

        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Theory]
    [InlineData("alllowercase1!")]
    [InlineData("ALLUPPERCASE1!")]
    [InlineData("NoNumber!")]
    [InlineData("NoSpecial123")]
    public void Validate_ShouldHaveError_WhenPasswordMissingRequiredComplexity(string password)
    {
        var dto = new UserRegisterDto
        {
            UserName = "user",
            Password = password,
            ConfirmedPassword = password
        };

        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }

    [Fact]
    public void Validate_ShouldFail_WhenPasswordsDoNotMatch()
    {
        var dto = new UserRegisterDto
        {
            UserName = "user",
            Password = "Valid123!",
            ConfirmedPassword = "Different123!"
        };

        var result = _validator.TestValidate(dto);
        result.ShouldHaveValidationErrorFor(x => x.ConfirmedPassword);
    }
}