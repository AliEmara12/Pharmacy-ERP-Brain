using System.Text.Json;
using PharmacyERP.Application.Common.Interfaces.Context;
using PharmacyERP.Application.Common.Interfaces.Integration;
using PharmacyERP.Application.Common.Utilities;
using PharmacyERP.Application.Features.Integration.Interfaces;
using PharmacyERP.Domain.Entities.Integration;
using PharmacyERP.Domain.Exceptions;

namespace PharmacyERP.Application.Features.Integration.Services.Implementations;

public class IdempotencyService : IIdempotencyService
{
    private readonly IIdempotencyRepository _repository;
    private readonly ITenantContext _tenantContext;

    public IdempotencyService(IIdempotencyRepository repository, ITenantContext tenantContext)
    {
        _repository = repository;
        _tenantContext = tenantContext;
    }

    public async Task<IdempotencyCheckResult<TResponse>> CheckAsync<TRequest, TResponse>(
        string operationScope, string idempotencyKey, Guid? branchId, TRequest requestPayload, CancellationToken ct = default)
    {
        if (!_tenantContext.OrganizationId.HasValue)
        {
            throw new InvalidOperationException("OrganizationId is required to perform idempotency checks.");
        }

        var orgId = _tenantContext.OrganizationId.Value;
        var payloadHash = CanonicalJsonHasher.ComputeSha256Hex(requestPayload);
        var existingRecord = await _repository.GetAsync(orgId, branchId, operationScope, idempotencyKey, ct);

        if (existingRecord != null)
        {
            if (existingRecord.PayloadHash != payloadHash)
            {
                throw new IntegrationDomainException("IDEMPOTENCY_MISMATCH");
            }

            var response = JsonSerializer.Deserialize<TResponse>(
                existingRecord.ResponseBody,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return new IdempotencyCheckResult<TResponse> { IsReplay = true, ReplayedResponse = response };
        }

        return new IdempotencyCheckResult<TResponse> { IsReplay = false };
    }

    public async Task PersistResponseAsync<TRequest, TResponse>(
        string operationScope, string idempotencyKey, Guid? branchId, TRequest requestPayload, TResponse responseBody, CancellationToken ct = default)
    {
        if (!_tenantContext.OrganizationId.HasValue)
        {
            throw new InvalidOperationException("OrganizationId is required to persist idempotency responses.");
        }

        var orgId = _tenantContext.OrganizationId.Value;
        var payloadHash = CanonicalJsonHasher.ComputeSha256Hex(requestPayload);
        var responseJson = JsonSerializer.Serialize(responseBody);

        var record = new IdempotencyRecord(orgId, branchId, operationScope, idempotencyKey, payloadHash, responseJson);
        await _repository.AddAsync(record, ct);
    }
}