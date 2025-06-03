using FtpApi.Application.DTOs;
using FtpApi.Application.Services;
using FtpApi.Data.Models;
using FtpApi.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FtpApi.Endpoints;

public static class FileEndpoints
{
    public static void MapFileEndpoints(this WebApplication app)
    {
        app.MapPost("/api/file", [Authorize] async ([FromForm] FileUploadDto input, HttpContext context) =>
        {
            using var scope = app.Services.CreateScope();
            CancellationTokenSource cts = new CancellationTokenSource();
            var ct = cts.Token;
            var userManager = scope.ServiceProvider.GetService<UserManager<ApiUser>>();
            string userId = await context.GetUserId(userManager);

            var service = scope.ServiceProvider.GetService<IFileUploadService>();
            await service.Upload(input, userId, ct);

            return Results.Ok("File uploaded.");
        })
            .DisableAntiforgery();

        app.MapDelete("/api/file/{filename}", [Authorize] async (string filename, HttpContext context) =>
        {
            using var scope = app.Services.CreateScope();
            CancellationTokenSource cts = new CancellationTokenSource();
            var ct = cts.Token;

            var userManager = scope.ServiceProvider.GetService<UserManager<ApiUser>>();
            string userId = await context.GetUserId(userManager);

            var service = scope.ServiceProvider.GetService<IFileDeleteService>();
            await service.Delete(userId, filename, ct);

            return Results.Ok("File deleted.");
        });

        app.MapGet("/api/file", [Authorize] async (HttpContext context) =>
        {
            using var scope = app.Services.CreateScope();
            CancellationTokenSource cts = new CancellationTokenSource();
            var ct = cts.Token;
            var userManager = scope.ServiceProvider.GetService<UserManager<ApiUser>>();
            string userId = await context.GetUserId(userManager);

            var service = scope.ServiceProvider.GetService<IFileGetFilesService>();
            var result = await service.GetAll(userId, ct);

            return Results.Ok(result);
        });

        app.MapGet("/api/file/{filename}", [Authorize] async (string filename, HttpContext context) =>
        {
            using var scope = app.Services.CreateScope();
            CancellationTokenSource cts = new CancellationTokenSource();
            var ct = cts.Token;

            var userManager = scope.ServiceProvider.GetService<UserManager<ApiUser>>();
            string userId = await context.GetUserId(userManager);

            var service = scope.ServiceProvider.GetService<IFileDownloadService>();
            var stream = await service.Download(userId, filename, ct);

            stream.Position = 0;

            return Results.File(stream);
        });
    }
}