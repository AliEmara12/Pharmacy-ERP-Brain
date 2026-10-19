using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PharmacyERP.Application.Common.Interfaces.Context;

namespace PharmacyERP.Infrastructure.Persistence.Interceptors;

public class TenantRlsConnectionInterceptor : DbCommandInterceptor
{
    private readonly ITenantContext _tenantContext;

    public TenantRlsConnectionInterceptor(ITenantContext tenantContext)
    {
        _tenantContext = tenantContext;
    }

    public override InterceptionResult<DbDataReader> ReaderExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
    {
        EnforceTenantSecurity(command);
        return base.ReaderExecuting(command, eventData, result);
    }

    public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result, CancellationToken cancellationToken = default)
    {
        EnforceTenantSecurity(command);
        return base.ReaderExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override InterceptionResult<int> NonQueryExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<int> result)
    {
        EnforceTenantSecurity(command);
        return base.NonQueryExecuting(command, eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> NonQueryExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        EnforceTenantSecurity(command);
        return base.NonQueryExecutingAsync(command, eventData, result, cancellationToken);
    }

    public override InterceptionResult<object> ScalarExecuting(DbCommand command, CommandEventData eventData, InterceptionResult<object> result)
    {
        EnforceTenantSecurity(command);
        return base.ScalarExecuting(command, eventData, result);
    }

    public override ValueTask<InterceptionResult<object>> ScalarExecutingAsync(DbCommand command, CommandEventData eventData, InterceptionResult<object> result, CancellationToken cancellationToken = default)
    {
        EnforceTenantSecurity(command);
        return base.ScalarExecutingAsync(command, eventData, result, cancellationToken);
    }

    private void EnforceTenantSecurity(DbCommand command)
    {
        if (_tenantContext.IsSystemBypassAllowed) return;

        if (command.Transaction == null)
            throw new InvalidOperationException("PostgreSQL RLS requires an active transaction. No transaction found.");

        if (!_tenantContext.IsTenantBound || _tenantContext.OrganizationId == null)
            throw new UnauthorizedAccessException("Tenant context is not bound. Cannot execute RLS-protected queries.");

        using var rlsCommand = command.Connection!.CreateCommand();
        rlsCommand.Transaction = command.Transaction;
        rlsCommand.CommandText = "SELECT set_config('app.organization_id', @orgId, true);";

        var param = rlsCommand.CreateParameter();
        param.ParameterName = "@orgId";
        param.Value = _tenantContext.OrganizationId.Value.ToString();
        rlsCommand.Parameters.Add(param);

        rlsCommand.ExecuteNonQuery();
    }
}