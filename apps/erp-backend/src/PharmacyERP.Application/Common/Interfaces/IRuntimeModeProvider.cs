

using PharmacyERP.Domain.Enums;

namespace PharmacyERP.Application.Common.Interfaces
{
    public interface IRuntimeModeProvider
    {
        ErpRuntimeMode CurrentMode { get; }
        Guid? EnrolledBranchId { get; }
    }
}
