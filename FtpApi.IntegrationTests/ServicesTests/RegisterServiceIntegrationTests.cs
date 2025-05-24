using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FtpApi.Application.DTOs;
using FtpApi.Application.Services;
using FtpApi.Application.Validators;
using FtpApi.Data;
using FtpApi.Data.Models;
using FtpApi.IntegrationTests.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace FtpApi.IntegrationTests.ServicesTests;

public class RegisterServiceIntegrationTests
{
    private readonly UserManager<ApiUser> _userManager;
    private readonly RegisterService _registerService;
    private readonly ServiceProvider _serviceProvider;

    public RegisterServiceIntegrationTests()
    {
        var loggerMocked = new Mock<ILogger<RegisterService>>();

        var services = new ServiceCollection();

        services.AddSingleton<DbConnection>(container =>
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            return connection;
        });

        services.AddLogging();
        services.AddDbContext<AppDbContext>((container, options) =>
        {
            var connection = container.GetRequiredService<DbConnection>();
            options.UseSqlite(connection, x => x.MigrationsAssembly("FtpApi.Migrations"));
        });



        services.AddIdentity<ApiUser, IdentityRole>()
            .AddEntityFrameworkStores<AppDbContext>();

        services.AddScoped<RegisterService>();
        services.AddScoped<IValidator<UserRegisterDto>, UserRegisterValidator>();

        _serviceProvider = services.BuildServiceProvider();

        _userManager = _serviceProvider.GetRequiredService<UserManager<ApiUser>>();
        var validator = _serviceProvider.GetRequiredService<IValidator<UserRegisterDto>>();
        var logger = loggerMocked.Object;

        var db = _serviceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
        db.Database.EnsureCreated();

        _registerService = new RegisterService(validator, _userManager, logger);
    }

    [Fact]
    public async Task Register_ShouldCreatesUserSuccessfully()
    {
        // Arrange
        var dto = new UserRegisterDto
        {
            UserName = "testuser",
            Password = "StrongP@ssword123",
            ConfirmedPassword = "StrongP@ssword123"
        };

        // Act
        var result = await _registerService.Register(dto);

        // Assert
        Assert.Equal(dto.UserName, result.UserName);
        var userInDb = await _userManager.FindByNameAsync(dto.UserName);
        Assert.NotNull(userInDb);
    }

    
    [Fact]
    public async Task Register_ShouldReturnError_WhenUserExists()
    {
        // Arrange
        var dto = new UserRegisterDto
        {
            UserName = "testuser",
            Password = "StrongP@ssword123",
            ConfirmedPassword = "StrongP@ssword123"
        };

        var dtoExists = new UserRegisterDto
        {
            UserName = "testuser",
            Password = "StrongP@ssword123",
            ConfirmedPassword = "StrongP@ssword123"
        };

        await _registerService.Register(dto);

        // Act
        // Assert
        await Assert.ThrowsAnyAsync<InvalidOperationException>(() => _registerService.Register(dtoExists));
    }

    public void Dispose()
    {
        _serviceProvider?.Dispose();
    }
}
