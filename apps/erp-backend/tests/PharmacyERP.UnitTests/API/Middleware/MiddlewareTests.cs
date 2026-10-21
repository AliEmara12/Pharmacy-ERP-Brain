using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using PharmacyERP.Api.Middleware;
using PharmacyERP.Application.Common.DTOs;
using PharmacyERP.Application.Common.Interfaces.Context;
using PharmacyERP.Domain.Exceptions;
using Xunit;

namespace PharmacyERP.UnitTests.Api.Middleware;

public class MiddlewareTests
{
    [Fact]
    public async Task TraceparentMiddleware_AddsTraceIdToHeadersAndItems()
    {
        var context = new DefaultHttpContext();
        var middleware = new TraceparentMiddleware(innerHttpContext => Task.CompletedTask);

        await middleware.InvokeAsync(context);

        Assert.True(context.Items.ContainsKey("TraceId"));
        Assert.True(context.Response.Headers.ContainsKey("X-Trace-Id"));
        Assert.NotNull(context.Items["TraceId"]);
    }

    [Fact]
    public async Task TenantResolutionMiddleware_SetsTenant_WhenUserIsAuthenticatedAndHasOrganizationClaim()
    {
        var orgId = Guid.NewGuid();
        var context = new DefaultHttpContext();
        var claims = new[]
        {
            new Claim("organization_id", orgId.ToString())
        };
        context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));

        var tenantContextMock = new Mock<ITenantContext>();
        var middleware = new TenantResolutionMiddleware(innerHttpContext => Task.CompletedTask);

        await middleware.InvokeAsync(context, tenantContextMock.Object);

        tenantContextMock.Verify(x => x.SetTenant(orgId), Times.Once);
    }

    [Fact]
    public async Task TenantResolutionMiddleware_DoesNotSetTenant_WhenUserIsUnauthenticated()
    {
        var context = new DefaultHttpContext();
        context.User = new ClaimsPrincipal(new ClaimsIdentity());

        var tenantContextMock = new Mock<ITenantContext>();
        var middleware = new TenantResolutionMiddleware(innerHttpContext => Task.CompletedTask);

        await middleware.InvokeAsync(context, tenantContextMock.Object);

        tenantContextMock.Verify(x => x.SetTenant(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task ExceptionHandlingMiddleware_CatchesDomainException_Returns422UnprocessableEntity()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();
        context.Items["TraceId"] = "test-trace-id";

        var loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        var middleware = new ExceptionHandlingMiddleware(
            innerHttpContext => throw new TestDomainException("Business rule violated."),
            loggerMock.Object
        );

        await middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var errorEnvelope = JsonSerializer.Deserialize<ApiErrorEnvelopeDto>(responseText, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        Assert.Equal((int)HttpStatusCode.UnprocessableEntity, context.Response.StatusCode);
        Assert.Equal("application/json", context.Response.ContentType);
        Assert.NotNull(errorEnvelope);
        Assert.Equal("test-trace-id", errorEnvelope.Error.TraceId);
        Assert.Equal("VALIDATION_ERROR", errorEnvelope.Error.Code);
        Assert.Equal("Business rule violated.", errorEnvelope.Error.Message);
    }

    [Fact]
    public async Task ExceptionHandlingMiddleware_CatchesGenericException_Returns500InternalServerError()
    {
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        var loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        var middleware = new ExceptionHandlingMiddleware(
            innerHttpContext => throw new Exception("Database connection failed."),
            loggerMock.Object
        );

        await middleware.InvokeAsync(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
        var errorEnvelope = JsonSerializer.Deserialize<ApiErrorEnvelopeDto>(responseText, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
        Assert.NotNull(errorEnvelope);
        Assert.Equal("INTERNAL_SERVER_ERROR", errorEnvelope.Error.Code);
    }

    private class TestDomainException : DomainException
    {
        public TestDomainException(string message) : base(message) { }
    }
}