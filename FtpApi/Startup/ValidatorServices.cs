using FluentValidation;
using FtpApi.Application.DTOs;
using FtpApi.Application.Validators;

namespace FtpApi.Startup;

public static class ValidatorServices
{
    public static void AddValidators(this IServiceCollection services)
    {
        services.AddScoped<IValidator<UserRegisterDto>, UserRegisterValidator>();
        services.AddScoped<IValidator<UserLoginDto>, UserLoginValidator>();
        services.AddScoped<IValidator<FileUploadDto>, FileUploadValidator>();
    }
}