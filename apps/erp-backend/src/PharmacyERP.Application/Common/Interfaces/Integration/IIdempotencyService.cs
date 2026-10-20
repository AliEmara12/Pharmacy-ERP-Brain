namespace PharmacyERP.Application.Common.Interfaces.Integration
{
    public interface IIdempotencyService
    {
        Task<IdempotencyCheckResult<TResponse>> CheckAsync<TRequest, TResponse>(string operationScope, string idempotencyKey, Guid? branchId, TRequest requestPayload, CancellationToken ct = default);
        Task PersistResponseAsync<TRequest, TResponse>(string operationScope, string idempotencyKey, Guid? branchId, TRequest requestPayload, TResponse responseBody, CancellationToken ct = default);
    }
    public record IdempotencyCheckResult<TResponse>(bool IsReplay, TResponse? CachedResponse);

}
