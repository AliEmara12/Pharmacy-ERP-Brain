using Microsoft.EntityFrameworkCore;

namespace PharmacyERP.Infrastructure.Persistence;

public sealed class PharmacyDbContext(DbContextOptions<PharmacyDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PharmacyDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
