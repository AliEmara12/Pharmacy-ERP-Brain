# Background Services

Contains asynchronous background workers:

- **Local/Cloud Sync Worker**: Manages offline-first edge syncing between branch pharmacy databases and central cloud instance.
- **Outbox Publisher**: Implements the Transactional Outbox pattern to safely publish integration events to message brokers without dual-write inconsistency.
