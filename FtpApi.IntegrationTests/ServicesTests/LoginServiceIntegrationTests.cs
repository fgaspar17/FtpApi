using System.Data.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentValidation;
using FtpApi.Application.DTOs;
using FtpApi.Application.Services;
using FtpApi.Application.Validators;
using FtpApi.Data;
using FtpApi.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace FtpApi.IntegrationTests.ServicesTests;

public class LoginServiceIntegrationTests
{
    private readonly UserManager<ApiUser> _userManager;
    private readonly RegisterService _registerService;
    private readonly ServiceProvider _serviceProvider;
    private readonly LoginService _loginService;
    private readonly IConfiguration _configuration;

    public LoginServiceIntegrationTests()
    {
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("appsettings.Tests.json")
            .Build();

        var registerLoggerMocked = new Mock<ILogger<RegisterService>>();
        var loginLoggerMocked = new Mock<ILogger<LoginService>>();

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

        services.AddScoped<LoginService>();
        services.AddScoped<IValidator<UserLoginDto>, UserLoginValidator>();

        _serviceProvider = services.BuildServiceProvider();

        _userManager = _serviceProvider.GetRequiredService<UserManager<ApiUser>>();
        var registerValidator = _serviceProvider.GetRequiredService<IValidator<UserRegisterDto>>();

        var loginValidator = _serviceProvider.GetRequiredService<IValidator<UserLoginDto>>();

        var db = _serviceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
        db.Database.EnsureCreated();

        _registerService = new RegisterService(registerValidator, _userManager, registerLoggerMocked.Object);
        _loginService = new LoginService(loginValidator, _userManager, loginLoggerMocked.Object);
    }

    [Fact]
    public async Task Login_ShouldSuccess_WhenUserIsValid()
    {
        // Arrange
        var registerDto = new UserRegisterDto
        {
            UserName = "testuser",
            Password = "StrongP@ssword123",
            ConfirmedPassword = "StrongP@ssword123"
        };
        await _registerService.Register(registerDto);
        var loginDto = new UserLoginDto
        {
            UserName = registerDto.UserName,
            Password = registerDto.Password
        };


        // Act
        var token = await _loginService.Login(loginDto, _configuration);
        var tokenHandler = new JwtSecurityTokenHandler();
        var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;


        // Assert
        Assert.NotNull(token);
        Assert.True(jwtToken.Claims.Any(c => c.Type == ClaimTypes.Name && c.Value == loginDto.UserName));
        Assert.Equal(jwtToken.Issuer, _configuration["JWT:Issuer"]);
        Assert.True(jwtToken.Audiences.Any(a => a == _configuration["JWT:Audience"]));
        Assert.True(jwtToken.ValidTo > DateTime.UtcNow);
        Assert.Equal(SecurityAlgorithms.HmacSha256, jwtToken.Header.Alg);
    }
}