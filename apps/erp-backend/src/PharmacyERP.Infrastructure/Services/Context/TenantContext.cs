using PharmacyERP.Application.Common.Interfaces.Context;

namespace PharmacyERP.Infrastructure.Services.Context;

public class TenantContext : ITenantContext
{
    public Guid? OrganizationId { get; private set; }
    public bool IsTenantBound => OrganizationId.HasValue;
    public bool IsSystemBypassEnabled { get; private set; }

    public void SetTenant(Guid organizationId)
    {
        OrganizationId = organizationId;
    }

    public void AllowSystemBypass()
    {
        IsSystemBypassEnabled = true;
    }
}