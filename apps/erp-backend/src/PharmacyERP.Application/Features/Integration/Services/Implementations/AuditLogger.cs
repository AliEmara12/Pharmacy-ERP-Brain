using System.Text.Json;
using PharmacyERP.Application.Common.Interfaces.Context;
using PharmacyERP.Application.Common.Interfaces.Integration;
using PharmacyERP.Application.Features.Integration.Interfaces;
using PharmacyERP.Domain.Entities.Integration;
using PharmacyERP.Domain.Enums;

namespace PharmacyERP.Application.Features.Integration.Services.Implementations;

public class AuditLogger : IAuditLogger
{
    private readonly IAuditLogRepository _repository;
    private readonly ITenantContext _tenantContext;
    private readonly ICurrentUser _currentUser;

    public AuditLogger(IAuditLogRepository repository, ITenantContext tenantContext, ICurrentUser currentUser)
    {
        _repository = repository;
        _tenantContext = tenantContext;
        _currentUser = currentUser;
    }

    public async Task LogAsync(
        string action,
        string entitySchema,
        string entityName,
        Guid entityId,
        object? beforeData,
        object? afterData,
        Guid? branchId = null,
        AuditActorType? actorTypeOverride = null,
        string? sourcePromptId = null,
        CancellationToken ct = default)
    {
        if (!_tenantContext.OrganizationId.HasValue)
        {
            throw new InvalidOperationException("OrganizationId is required for audit logs.");
        }

        var orgId = _tenantContext.OrganizationId.Value;
        var actorType = actorTypeOverride ?? (_currentUser.IsAuthenticated ? AuditActorType.USER : AuditActorType.SYSTEM);

        var beforeJson = beforeData != null ? JsonSerializer.Serialize(beforeData) : null;
        var afterJson = afterData != null ? JsonSerializer.Serialize(afterData) : null;

        var logEntry = new AuditLog(
            orgId,
            branchId,
            _currentUser.UserId,
            _currentUser.EmployeeId,
            actorType,
            action,
            entitySchema,
            entityName,
            entityId,
            beforeJson,
            afterJson,
            _currentUser.TraceId,
            sourcePromptId
        );

        await _repository.AddAsync(logEntry, ct);
    }
}