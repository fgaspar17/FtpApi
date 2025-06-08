using FtpApi.Application.Services;

namespace FtpApi.Startup;

public static class FileFtpServices
{
    public static void AddFileFtpServices(this IServiceCollection services)
    {
        services.AddScoped<IFileUploadService, FileUploadFtpService>();
        services.AddScoped<IFileDeleteService, FileDeleteFtpService>();
        services.AddScoped<IFileGetFilesService, FileGetFilesFtpService>();
        services.AddScoped<IFileDownloadService, FileDownloadFtpService>();
    }
}