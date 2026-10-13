# Taktera.Mail — Proposed local database design

Status: **Proposed**. Implementation requires approval of the local-storage ADR.

## Core entities

- `Accounts` — local account metadata only; no passwords/tokens.
- `Folders` — server folders/special-use metadata.
- `Messages` — local message identity and common metadata.
- `FolderMessages` — remote location identity: account + folder + UIDVALIDITY + UID.
- `Attachments` — attachment metadata and local-cache location.
- `Drafts` — local draft state.
- `Outbox` — queued/sending/sent/failed/unknown delivery state.
- `PendingOperations` — offline IMAP actions awaiting replay.
- `SyncState` — per-account/folder checkpoints and capability state.

## Identity rules

`Message-Id` is not a primary key. It can be absent or duplicated. Server identity is scoped by account/folder and IMAP UIDVALIDITY/UID. Local IDs remain stable across cache operations and permit Gmail-like multi-folder/label mappings.

## SQLite behavior

- `PRAGMA foreign_keys = ON`.
- WAL is proposed, with a controlled writer strategy.
- Versioned, data-preserving migrations.
- Transactions for data plus synchronization checkpoint commits.
- Handle `SQLITE_BUSY` explicitly; WAL does not remove the single-writer constraint.
- Bodies and large attachments loaded/stored on demand according to cache policy.

## FTS5

Proposed external-content index over subject, normalized addresses and extracted text body. Start with `unicode61`; verify Cyrillic/Latin search, prefix behavior and native SQLite FTS5 availability on Windows/macOS. User text must be translated into a constrained FTS query grammar; SQL parameters alone do not validate MATCH syntax.

## Security

Secrets never belong in SQLite. Local-mail encryption policy, key management and recovery remain an ADR decision. Diagnostics must not dump message bodies by default.

## Required tests

Migration replay/rollback behavior, insert-update-delete consistency, FTS rebuild/backfill, Cyrillic search, account isolation, crash between data/checkpoint operations, busy database handling, cache deletion and account cleanup.
