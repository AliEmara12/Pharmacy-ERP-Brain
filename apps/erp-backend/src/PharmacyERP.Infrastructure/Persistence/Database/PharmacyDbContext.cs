using Microsoft.EntityFrameworkCore;

namespace PharmacyERP.Infrastructure.Persistence.Database;

public partial class PharmacyDbContext(DbContextOptions<PharmacyDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PharmacyDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
