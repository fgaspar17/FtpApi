using FtpApi.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FtpApi.Data;

public class AppDbContext : IdentityDbContext<ApiUser>
{
    public DbSet<FileMetadata> FileMetadatas { get; set; }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<FileMetadata>()
            .HasOne<ApiUser>(fm => fm.ApiUser)
            .WithMany(u => u.FileMetadatas)
            .HasForeignKey(fm => fm.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<FileMetadata>()
            .HasQueryFilter(fm => !fm.IsDeleted);

        modelBuilder.Entity<FileMetadata>()
            .Property(fm => fm.UploadedAtUtc)
            .HasDefaultValueSql("datetime('now', 'utc')");
    }
}
