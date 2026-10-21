using Microsoft.AspNetCore.Http;

namespace PharmacyERP.Api.Middleware;

public class TraceparentMiddleware
{
    private readonly RequestDelegate _next;
    private const string TraceIdHeaderName = "X-Trace-Id";

    public TraceparentMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var traceId = context.Request.Headers[TraceIdHeaderName].FirstOrDefault() ?? Guid.NewGuid().ToString();

        context.Items["TraceId"] = traceId;
        context.Response.Headers[TraceIdHeaderName] = traceId;

        await _next(context);
    }
}