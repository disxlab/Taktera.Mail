# Taktera.Mail — Technical Specification

Canonical source: project documentation package approved for repository import.

## Scope

Windows-first, macOS-second desktop email client built with C#/.NET 10 and Avalonia UI. Architecture follows Desktop → Application → Domain with Infrastructure adapters added only after ADR approval.

## Core requirements

- Multiple accounts and folders.
- Offline-first reading and local cache.
- SQLite + FTS5 for local storage/search after ADR approval.
- MailKit/MimeKit for IMAP/SMTP and MIME processing after ADR approval.
- OAuth2 Authorization Code + PKCE for supported providers after ADR approval and separate permission for real accounts.
- Safe HTML rendering, remote content blocked by default.
- Reliable drafts and outbox with explicit `unknown` state after ambiguous SMTP delivery.
- Windows and macOS support, CI on both platforms.

## Architecture boundaries

### Domain
Pure business entities, typed IDs, message/folder/account state, no Avalonia, SQLite, MailKit, OAuth SDKs.

### Application
Use cases and contracts for repositories, transport, search, sync, auth, clock and secret storage.

### Infrastructure
Concrete MailKit/MimeKit, SQLite/FTS5, OAuth provider and platform adapters. Must not be introduced before corresponding ADR approval.

### Desktop
Avalonia Views, ViewModels, navigation and composition root. No direct SQL or IMAP in Views/ViewModels.

## Data identity

Do not use Message-Id as a unique primary key. IMAP identity is account + folder + UIDVALIDITY + UID. Keep local message identity independent from remote server identity.

## Synchronization

- Fetch folders and headers in pages.
- Bodies/attachments on demand.
- Persist UIDVALIDITY, UID and MODSEQ where supported.
- Use CONDSTORE/QRESYNC when available, otherwise bounded reconciliation.
- IDLE when supported; polling with backoff/jitter otherwise.
- Commit data and sync checkpoint atomically.
- Queue offline operations separately and make replay idempotent where possible.

## Search

- SQLite FTS5 external-content strategy proposed.
- Index subject, addresses and normalized text body.
- Start with `unicode61` tokenizer; no morphology guarantees.
- Validate MATCH grammar separately from SQL parameterization.
- Support account/folder filtering and stable pagination.

## Security

- Never store tokens/passwords in SQLite or logs.
- Use platform secure stores after ADR approval.
- TLS validation must stay enabled.
- Treat HTML, attachments and URLs as untrusted.
- Block scripts, remote images/resources and auto-open attachments by default.
- Use parameterized SQL and bounded parsing limits.

## Performance targets

Proposed targets on a defined Windows x64 SSD/16 GB RAM baseline:

- cold start to list ≤ 2s;
- search p95 ≤ 300 ms on 100,000 indexed messages;
- idle working set ≤ 200 MB.

These are targets, not claims about current implementation.

## Release sequence

M00 baseline → M01 Domain/Application → M02 UI → M03 SQLite/Search → M04 MIME → M05 IMAP → M06 OAuth/accounts → M07 Compose/SMTP → M08 feature complete → M09 hardening → M10 platform integration → M11 alpha/beta/RC → M12 production.

See `ISSUES_FULL.md`, `API_CONTRACTS.md`, `DATABASE_DESIGN.md`, `DATABASE_SCHEMA.sql`, `INTERFACE_SPEC.md`, `DESIGN_SYSTEM.md`, and `CODEX_PHASE_PROMPTS.md` for detailed work packages.
