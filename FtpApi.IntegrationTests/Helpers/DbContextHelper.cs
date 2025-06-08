using FtpApi.Data;
using Microsoft.EntityFrameworkCore;

namespace FtpApi.IntegrationTests.Helpers;

public static class DbContextHelper
{
    public static AppDbContext GetContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>().
            UseSqlite("Data Source=:memory:")
            .Options;

        var db = new AppDbContext(options);

        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

        return db;
    }
}