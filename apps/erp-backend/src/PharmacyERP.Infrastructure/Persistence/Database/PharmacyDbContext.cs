using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Common.Interfaces.Context;
using System.Linq.Expressions;

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
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, orgIdProperty.PropertyInfo!);
                var currentOrgId = Expression.Property(
                    Expression.Constant(this),
                    nameof(CurrentOrganizationId)
                );

                var filter = Expression.Equal(property, Expression.Convert(currentOrgId, typeof(Guid)));
                var lambda = Expression.Lambda(filter, parameter);

                entityType.SetQueryFilter(lambda);
            }
        }
    }
}