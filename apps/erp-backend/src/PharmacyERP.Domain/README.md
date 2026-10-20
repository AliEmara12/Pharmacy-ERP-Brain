# Domain Layer

The enterprise core containing pure business logic, free from database or UI dependencies:

- **Entities/**: Aggregate roots and entities partitioned by bounded context (Identity, Catalog, Inventory, Sales, Purchasing).
- **ValueObjects/**: Immutable objects representing concepts without identity (e.g. `Money`, `BatchInfo`, `Dosage`, `Barcode`).
- **Enums/**: Domain state enumerations (`OrderStatus`, `PaymentType`, `InventoryMovementType`).
- **Invariants/**: Pure domain rules, specifications, and constraint checks (e.g. expiration date must be in future, inventory cannot go negative).
- **Exceptions/**: Custom domain exceptions thrown when invariant checks fail.
