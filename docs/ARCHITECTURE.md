# Architecture

Current references: Desktop → Application → Domain; Infrastructure → Application → Domain.

- Desktop: Avalonia UI / MVVM demo, Windows-first. macOS compatibility is not verified yet.
- Application: account/folder synchronization contracts with cancellation; no implementations.
- Domain: MailAccount, MailFolder, MailMessage metadata, ID validation, UTC dates and read state.
- Infrastructure: empty buildable project for future adapters; no external packages.
- Tests: xUnit domain tests independent of UI, network and real accounts.

Current milestone is a demo UI, not a functioning mail service.

Desktop still uses Desktop.Models.MailItem and synthetic messages. It does not reference
Infrastructure. No DI container or background worker is added. Bodies, attachments,
IMAP identities and folder membership will be designed with persistence; MailMessage
does not imply a unique server Message-Id or a single folder.

MailKit/MimeKit, SQLite FTS5 and OAuth2 adapters remain planned. Graph is an optional
later decision. See [ADR-001](ADR-001-foundation.md) and [implementation plan](IMPLEMENTATION_PLAN.md).
