using System.Data.Common;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using FluentValidation;
using FtpApi.Application.DTOs;
using FtpApi.Application.Services;
using FtpApi.Application.Utils;
using FtpApi.Application.Validators;
using FtpApi.Data;
using FtpApi.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FtpApi.IntegrationTests.Helpers;

public class FtpTestFixture : IAsyncLifetime
{
    public IServiceProvider ServiceProvider { get; private set; } = null!;
    public string UserId { get; private set; } = null!;
    public FileUploadFtpService UploadService { get; private set; } = null!;
    public FileDeleteFtpService DeleteService { get; private set; } = null!;
    public FileDownloadFtpService DownloadService { get; private set; } = null!;
    public FileGetFilesFtpService GetAllService { get; private set; } = null!;

    private IContainer _ftpServer = new ContainerBuilder()
        .WithImage("fauria/vsftpd")
        .WithEnvironment("FTP_USER", "test")
        .WithEnvironment("FTP_PASS", "test")
        .WithEnvironment("PASV_ENABLE", "YES")
        .WithEnvironment("PASV_MIN_PORT", "21100")
        .WithEnvironment("PASV_MAX_PORT", "21101")
        .WithEnvironment("PASV_ADDRESS", "127.0.0.1")
        .WithPortBinding(21, 21)
        .WithPortBinding(21100, 21100)
        .WithPortBinding(21101, 21101)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(21))
        .Build();

    private readonly SqliteConnection _sharedConnection = new SqliteConnection("DataSource=:memory:");

    public async Task InitializeAsync()
    {
        await _ftpServer.StartAsync();

        var config = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("appsettings.Tests.json")
            .Build();

        var services = new ServiceCollection();

        services.AddSingleton<DbConnection>(_ =>
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            return connection;
        });

        services.AddLogging();
        _sharedConnection.Open();

        services.AddSingleton<DbConnection>(_ => _sharedConnection);
        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var connection = sp.GetRequiredService<DbConnection>();
            options.UseSqlite(connection, x => x.MigrationsAssembly("FtpApi.Migrations"));
        });

        services.AddIdentity<ApiUser, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>();

        services.AddScoped<IValidator<FileUploadDto>, FileUploadValidator>();
        services.AddScoped<FtpUtils>();
        services.AddScoped<FileUploadFtpService>();
        services.AddScoped<FileDeleteFtpService>();
        services.AddScoped<FileDownloadFtpService>();
        services.AddScoped<FileGetFilesFtpService>();

        services.Configure<FtpApi.Application.Config.FtpConfig>(opt =>
        {
            opt.Host = _ftpServer.Hostname;
            opt.Port = _ftpServer.GetMappedPublicPort(21);
            opt.Username = config["FtpConfig:Username"];
            opt.Password = config["FtpConfig:Password"];
        });

        ServiceProvider = services.BuildServiceProvider();

        var db = ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
        db.Database.EnsureCreated();

        // Seed test user
        UserId = Guid.NewGuid().ToString();
        var testUser = new ApiUser { Id = UserId, UserName = "testuser" };
        db.Users.Add(testUser);
        await db.SaveChangesAsync();

        // Resolve services
        UploadService = ServiceProvider.GetRequiredService<FileUploadFtpService>();
        DeleteService = ServiceProvider.GetRequiredService<FileDeleteFtpService>();
        DownloadService = ServiceProvider.GetRequiredService<FileDownloadFtpService>();
        GetAllService = ServiceProvider.GetRequiredService<FileGetFilesFtpService>();
    }

    public async Task DisposeAsync()
    {
        await _ftpServer.DisposeAsync().AsTask();
        await _sharedConnection.DisposeAsync();
    }
}