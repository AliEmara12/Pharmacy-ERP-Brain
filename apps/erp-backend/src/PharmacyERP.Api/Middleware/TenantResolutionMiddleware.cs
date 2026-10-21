using PharmacyERP.Application.Common.Interfaces.Context;

namespace PharmacyERP.Api.Middleware;

public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var orgIdClaim = context.User.FindFirst("organization_id")?.Value;

            if (Guid.TryParse(orgIdClaim, out var organizationId))
            {
                tenantContext.SetTenant(organizationId);
            }
        }

        await _next(context);
    }
}