# Persistence Layer

Contains Entity Framework Core database access components:

- **PharmacyDbContext**: Primary EF Core DbContext managing database transactions and change tracking.
- **Configurations/**: One `IEntityTypeConfiguration<TEntity>` per entity defining tables, primary keys, relationships, indexes, and column types.
- **Repositories/**: Generic repository base implementations and workflow-specific repositories (e.g., `ISalesOrderRepository`, `IInventoryBatchRepository`).
- **Migrations/**: EF Core migration snapshots and history.
