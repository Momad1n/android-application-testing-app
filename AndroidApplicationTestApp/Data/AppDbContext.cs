using Backend.Domain;
using Microsoft.EntityFrameworkCore;

namespace AATP.Backend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<TestRun> TestRuns => Set<TestRun>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<TestRun>(entity =>
        {
            entity.Property(x => x.Status)
                  .HasConversion<int>();

            entity.Property(x => x.CreatedDate)
                  .IsRequired();

            entity.Property(x => x.Status)
                  .HasDefaultValue(TestRunStatus.Pending);
        });
    }
}
