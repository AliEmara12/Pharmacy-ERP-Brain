using System.Text.Json;
using PharmacyERP.Application.Common.Interfaces.Context;
using PharmacyERP.Application.Common.Interfaces.Integration;
using PharmacyERP.Application.Features.Integration.Interfaces;
using PharmacyERP.Domain.Entities.Integration;

namespace PharmacyERP.Application.Features.Integration.Services.Implementations;

public class OutboxService : IOutboxService
{
    private readonly IOutboxRepository _repository;
    private readonly ITenantContext _tenantContext;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICurrentUser _currentUser;

    public OutboxService(IOutboxRepository repository, ITenantContext tenantContext, IDateTimeProvider dateTimeProvider, ICurrentUser currentUser)
    {
        _repository = repository;
        _tenantContext = tenantContext;
        _dateTimeProvider = dateTimeProvider;
        _currentUser = currentUser;
    }

    public async Task EnqueueAsync<TPayload>(string eventType, Guid? branchId, TPayload payload, string schemaVersion = "1.0", CancellationToken ct = default)
    {
        if (!_tenantContext.OrganizationId.HasValue)
        {
            throw new InvalidOperationException("OrganizationId is required to enqueue outbox events.");
        }

        var orgId = _tenantContext.OrganizationId.Value;

        var cursor = await _repository.LockAndGetCursorAsync(orgId, ct);
        if (cursor == null)
        {
            throw new InvalidOperationException("OutboxEventCursor missing. Ensure PlatformProvisioner created the initial cursor.");
        }

        var nextSequence = cursor.AllocateNextSequence(_dateTimeProvider.UtcNow);
        var payloadJson = JsonSerializer.Serialize(payload);

        var outboxEvent = new OutboxEvent(
            Guid.NewGuid(),
            orgId,
            branchId,
            nextSequence,
            eventType,
            payloadJson,
            _currentUser.TraceId
        );

        await _repository.AddEventAsync(outboxEvent, ct);
    }
}