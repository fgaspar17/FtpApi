using FtpApi.Application.Utils;
using FtpApi.Data;
using FtpApi.Data.Models;
using FtpApi.Endpoints;
using FtpApi.Middlewares;
using FtpApi.Startup;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApiServices();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("AppDbContext"),
    x => x.MigrationsAssembly("FtpApi.Migrations")));

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddIdentity<ApiUser, IdentityRole>()
 .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddAuthenticationServices(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddValidators();

builder.Services.AddScoped<FtpUtils>();
builder.Services.Configure<FtpApi.Application.Config.FtpConfig>(builder.Configuration.GetSection("FtpConfig"));
builder.Services.AddFileFtpServices();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApi();
}

app.UseExceptionHandler("/error");

app.UseMiddleware<RequestLogContextMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.MapErrorEndpoint();
app.MapAuthentication(app.Configuration);
app.MapFileEndpoints();

app.Run();