using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces.Context;

namespace PharmacyERP.Infrastructure.Persistence.Database;

public partial class PharmacyDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;

    public Guid? CurrentOrganizationId => _tenantContext.OrganizationId;

    public PharmacyDbContext(DbContextOptions<PharmacyDbContext> options, ITenantContext tenantContext)
        : base(options)
    {
        _tenantContext = tenantContext;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PharmacyDbContext).Assembly);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var foreignKeys = entityType.GetForeignKeys().Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);
            foreach (var fk in foreignKeys)
            {
                fk.DeleteBehavior = DeleteBehavior.Restrict;
            }

            var orgIdProperty = entityType.FindProperty("OrganizationId");
            if (orgIdProperty != null && orgIdProperty.ClrType == typeof(Guid))
            {
                var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                var property = System.Linq.Expressions.Expression.Property(parameter, orgIdProperty.PropertyInfo!);
                var currentOrgId = System.Linq.Expressions.Expression.Property(
                    System.Linq.Expressions.Expression.Constant(this),
                    nameof(CurrentOrganizationId)
                );

                var filter = System.Linq.Expressions.Expression.Equal(property, System.Linq.Expressions.Expression.Convert(currentOrgId, typeof(Guid)));
                var lambda = System.Linq.Expressions.Expression.Lambda(filter, parameter);

                entityType.SetQueryFilter(lambda);
            }
        }
    }
}