using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PharmacyERP.Application.Common.Interfaces.Context;
using PharmacyERP.Application.Common.Interfaces.Persistence;
using PharmacyERP.Infrastructure.Persistence.Database;

namespace PharmacyERP.Infrastructure.Persistence.UoW;

public class UnitOfWork : IUnitOfWork
{
    private readonly PharmacyDbContext _dbContext;
    private readonly ITenantContext _tenantContext;
    private IDbContextTransaction? _currentTransaction;

    public UnitOfWork(PharmacyDbContext dbContext, ITenantContext tenantContext)
    {
        _dbContext = dbContext;
        _tenantContext = tenantContext;
    }

    public bool HasActiveTransaction => _currentTransaction != null;

    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        if (_currentTransaction != null)
            throw new InvalidOperationException("A transaction is already active in this UnitOfWork.");

        _currentTransaction = await _dbContext.Database.BeginTransactionAsync(ct);

        if (_tenantContext.IsTenantBound && _tenantContext.OrganizationId.HasValue && _dbContext.Database.IsRelational())
        {
            var orgId = _tenantContext.OrganizationId.Value.ToString();
            await _dbContext.Database.ExecuteSqlRawAsync(
                "SELECT set_config('app.organization_id', {0}, true);",
                new object[] { orgId },
                ct);
        }
    }

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => _dbContext.SaveChangesAsync(ct);

    public async Task CommitTransactionAsync(CancellationToken ct = default)
    {
        if (_currentTransaction == null)
            throw new InvalidOperationException("No active transaction to commit.");

        try
        {
            await _dbContext.SaveChangesAsync(ct);
            await _currentTransaction.CommitAsync(ct);
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken ct = default)
    {
        if (_currentTransaction == null) return;

        try
        {
            await _currentTransaction.RollbackAsync(ct);
        }
        finally
        {
            await _currentTransaction.DisposeAsync();
            _currentTransaction = null;
        }
    }

    public void Dispose()
    {
        _currentTransaction?.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        if (_currentTransaction != null)
            await _currentTransaction.DisposeAsync();
    }
}