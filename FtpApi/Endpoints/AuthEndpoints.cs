using FluentValidation;
using FtpApi.Application.DTOs;
using FtpApi.Application.Services;
using FtpApi.Data.Models;
using Microsoft.AspNetCore.Identity;

namespace FtpApi.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthentication(this WebApplication app, IConfiguration config)
    {
        app.MapPost("/api/login", async (UserLoginDto input) =>
        {
            using var scope = app.Services.CreateScope();
            var service = new LoginService(
                scope.ServiceProvider.GetService<AbstractValidator<UserLoginDto>>(),
                scope.ServiceProvider.GetService<UserManager<ApiUser>>(),
                scope.ServiceProvider.GetService<ILogger<LoginService>>()
                );

            var token = await service.Login(input, config);

            return new { Token = token };
        });

        app.MapPost("/api/register", async (UserRegisterDto input) =>
        {
            using var scope = app.Services.CreateScope();
            var service = new RegisterService(
                scope.ServiceProvider.GetService<AbstractValidator<UserRegisterDto>>(),
                scope.ServiceProvider.GetService<UserManager<ApiUser>>(),
                scope.ServiceProvider.GetService<ILogger<RegisterService>>()
                );

            var newUser = await service.Register(input);

            return Results.Ok($"User '{newUser.UserName}' has been created.");
        });
    }
}
