using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

using Backend.Domain;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<TestRun> TestRuns => Set<TestRun>();
    public DbSet<TestScenario> TestScenarios { get; set; }
    public DbSet<TestApplication> TestApplications { get; set; }
    public DbSet<TestConfiguration> TestConfigurations { get; set; }

    public DbSet<EmulatorConfiguration> EmulatorConfigurations { get; set; }

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
