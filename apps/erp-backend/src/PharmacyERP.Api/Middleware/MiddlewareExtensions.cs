
namespace PharmacyERP.Api.Middleware;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UsePharmacyErpMiddlewares(this IApplicationBuilder app)
    {
        // Order is critical: 
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.UseMiddleware<TraceparentMiddleware>();

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseMiddleware<TenantResolutionMiddleware>();

        app.UseAuthorization();


        return app;
    }
}