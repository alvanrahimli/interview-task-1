using IT.Encryption.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace IT.Encryption.Data;

public class AppDbContext : DbContext
{
    public DbSet<EncryptionKey> Keys => Set<EncryptionKey>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EncryptionKey>()
            .HasIndex(k => k.Key);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        UpdateTimeStamps(ChangeTracker.Entries());
        return base.SaveChangesAsync(cancellationToken);
    }

    private static void UpdateTimeStamps(IEnumerable<EntityEntry> entries)
    {
        var filtered = entries.Where(x => x.State is EntityState.Added or EntityState.Modified or EntityState.Deleted);
        foreach (var entry in filtered)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    ((EntityAudit)entry.Entity).CreatedAt = DateTime.UtcNow;
                    ((EntityAudit)entry.Entity).UpdatedAt = DateTime.UtcNow;
                    break;
                case EntityState.Deleted when entry.Entity is EntityAudit auditEntity:
                    auditEntity.Deleted = true;
                    auditEntity.DeletedAt = DateTime.UtcNow;
                    break;
                case EntityState.Modified:
                    ((EntityAudit)entry.Entity).UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }
    }
}