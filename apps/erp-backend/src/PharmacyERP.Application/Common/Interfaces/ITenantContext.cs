

namespace PharmacyERP.Application.Common.Interfaces
{
    public interface ITenantContext
    {
        Guid? OrganizationId { get; }
        bool IsTenantBound { get; }
        bool IsSystemBypassAllowed { get; }
        void SetTenant(Guid organizationId);
        void AllowSystemBypass();
    }
}
