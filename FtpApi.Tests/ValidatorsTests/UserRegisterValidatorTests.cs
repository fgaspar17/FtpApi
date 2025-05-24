using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.TestHelper;
using FtpApi.Application.DTOs;
using FtpApi.Application.Validators;

namespace FtpApi.Tests.ValidatorsTests;

public class UserRegisterValidatorTests
{
    private readonly UserRegisterValidator _validator = new();

    [Fact]
    public void Should_Pass_With_Valid_Input()
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
    public void Should_Fail_When_UserName_Is_Empty()
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
    public void Should_Fail_When_Password_Too_Short()
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
    public void Should_Fail_When_Password_Missing_Required_Complexity(string password)
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
    public void Should_Fail_When_Passwords_Do_Not_Match()
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
