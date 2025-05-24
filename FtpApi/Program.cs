using FluentValidation;
using FtpApi.Application.DTOs;
using FtpApi.Application.Validators;
using FtpApi.Data;
using FtpApi.Data.Models;
using FtpApi.Endpoints;
using FtpApi.Middlewares;
using FtpApi.Startup;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("AppDbContext"), 
    x => x.MigrationsAssembly("FtpApi.Migrations")));

builder.Services.AddIdentity<ApiUser, IdentityRole>()
 .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddAuthenticationServices(builder.Configuration);

builder.Services.AddScoped<AbstractValidator<UserRegisterDto>, UserRegisterValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<RequestLogContextMiddleware>();

app.UseAuthentication();

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.MapAuthentication(app.Logger);

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
