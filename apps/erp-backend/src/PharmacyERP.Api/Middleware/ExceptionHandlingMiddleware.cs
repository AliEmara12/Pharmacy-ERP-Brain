using System.Net;
using System.Text.Json;
using PharmacyERP.Application.Common.DTOs;
using PharmacyERP.Domain.Exceptions;

namespace PharmacyERP.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred during request processing.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = HttpStatusCode.InternalServerError;
        var errorCode = "INTERNAL_SERVER_ERROR";
        var message = "An unexpected error occurred.";

        switch (exception)
        {
            case DomainException domainEx:
                statusCode = HttpStatusCode.UnprocessableEntity;
                errorCode = "VALIDATION_ERROR";
                message = domainEx.Message;
                break;
            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Forbidden;
                errorCode = "FORBIDDEN";
                message = "You do not have permission to perform this action.";
                break;
            case InvalidOperationException invalidOpEx:
                statusCode = HttpStatusCode.BadRequest;
                errorCode = "BAD_REQUEST";
                message = invalidOpEx.Message;
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var traceId = context.Items["TraceId"]?.ToString() ?? Guid.NewGuid().ToString();

        var errorBody = new ApiErrorBodyDto(errorCode, message, new List<ApiErrorDetailDto>(),traceId);
        var errorEnvelope = new ApiErrorEnvelopeDto(errorBody);

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var result = JsonSerializer.Serialize(errorEnvelope, options);

        await context.Response.WriteAsync(result);
    }
}