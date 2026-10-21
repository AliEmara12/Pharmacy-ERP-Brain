using Microsoft.EntityFrameworkCore;
using PharmacyERP.Application.Features.Integration.Interfaces;
using PharmacyERP.Domain.Entities.Integration;
using PharmacyERP.Infrastructure.Persistence.Database;

namespace PharmacyERP.Infrastructure.Persistence.Repositories.Integration;

public class OutboxRepository : IOutboxRepository
{
    private readonly PharmacyDbContext _dbContext;

    public OutboxRepository(PharmacyDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<OutboxEventCursor?> LockAndGetCursorAsync(Guid organizationId, CancellationToken ct = default)
    {
        var cursors = await _dbContext.OutboxEventCursors
            .FromSqlInterpolated($"SELECT * FROM integration.\"OUTBOX_EVENT_CURSOR\" WHERE organization_id = {organizationId} FOR UPDATE")
            .ToListAsync(ct);

        return cursors.FirstOrDefault();
    }

    public async Task AddEventAsync(OutboxEvent outboxEvent, CancellationToken ct = default)
    {
        await _dbContext.OutboxEvents.AddAsync(outboxEvent, ct);
    }
}