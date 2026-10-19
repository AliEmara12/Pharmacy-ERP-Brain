using Microsoft.AspNetCore.Http;
using PharmacyERP.Application.Common.Interfaces.Context;
using System.Security.Claims;

namespace PharmacyERP.Infrastructure.Services.Context;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

    public Guid? UserId => Guid.TryParse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User?.FindFirst("sub")?.Value, out var id) ? id : null;

    public Guid? EmployeeId => Guid.TryParse(User?.FindFirst("employee_id")?.Value, out var id) ? id : null;

    public Guid? OrganizationId => Guid.TryParse(User?.FindFirst("organization_id")?.Value, out var id) ? id : null;

    public string? Role => User?.FindFirst(ClaimTypes.Role)?.Value ?? User?.FindFirst("roles")?.Value;

    public string? BranchScope => User?.FindFirst("branch_scope")?.Value;

    public IReadOnlySet<Guid> AuthorizedBranchIds => User?.FindAll("branch_ids")
        .Select(c => Guid.TryParse(c.Value, out var id) ? id : Guid.Empty)
        .Where(id => id != Guid.Empty)
        .ToHashSet() ?? new HashSet<Guid>();

    public IReadOnlySet<string> Scopes => User?.FindAll("scopes")
        .Select(c => c.Value)
        .ToHashSet() ?? new HashSet<string>();

    public string TraceId => _httpContextAccessor.HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString();

    public bool CanAccessBranch(Guid branchId)
    {
        if (!IsAuthenticated) return false;

        if (Role == "Admin" && BranchScope == "ALL") return true;

        return AuthorizedBranchIds.Contains(branchId);
    }
}