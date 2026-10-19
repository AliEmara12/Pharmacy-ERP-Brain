using PharmacyERP.Application.Common.Interfaces.Context;

namespace PharmacyERP.Infrastructure.Services.Context;

public class TenantContext : ITenantContext
{
    public Guid? OrganizationId { get; private set; }
    public bool IsTenantBound => OrganizationId.HasValue;
    public bool IsSystemBypassAllowed { get; private set; }

    public void SetTenant(Guid organizationId)
    {
        if (IsTenantBound && OrganizationId != organizationId)
            throw new InvalidOperationException("Tenant is already set for this context and cannot be changed during the same request.");

        OrganizationId = organizationId;
    }

    public void AllowSystemBypass()
    {
        IsSystemBypassAllowed = true;
    }
}