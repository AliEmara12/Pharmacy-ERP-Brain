namespace PharmacyERP.Application.Common.Interfaces.Context;

public interface ITenantContext
{
    Guid? OrganizationId { get; }
    bool IsTenantBound { get; }
    bool IsSystemBypassEnabled { get; }
    void SetTenant(Guid organizationId);
    void AllowSystemBypass();
}