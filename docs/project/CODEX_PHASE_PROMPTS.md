# Taktera.Mail — Codex phase instructions

This document is the execution guide for Codex. It does not override `AGENTS.md` or approved ADRs.

## Global rules for every phase

Before changing code:

1. Read `AGENTS.md`.
2. Read `docs/project/README.md` and all project docs relevant to the phase.
3. Inspect `git status`, current branch, open PRs/issues and existing changes.
4. Work in a dedicated `codex/<task>` branch.
5. Preserve unrelated user changes.
6. Never merge to `main` without explicit user authorization.
7. Never connect real mail accounts or perform real OAuth/mail sending without separate explicit permission.
8. Architecture changes require approved ADR before implementation.
9. Run the required Release build/tests and report exact commands/results.

## Phase 00 — Engineering Baseline

Work only on `TM-00-*` issues. Establish reproducible builds, dependency audit, SDK/build standardization, tests and CI. Prepare Proposed ADRs where needed. Do not add Infrastructure implementations until ADR approval. Fix only the identified demo layout defects that are explicitly part of baseline work.

## Phase 01 — Domain & Application

After Phase 00 acceptance and architecture approval, implement typed IDs, domain entities, Application contracts/use cases and fixture/in-memory adapters. No real network or SQLite. Ensure Domain has no external infrastructure dependencies. Add unit tests for invariants, cancellation and error states.

## Phase 02 — UI & Design System

Implement the approved Avalonia design system and offline UI against fixture/Application services. Add responsive panes, virtualization/paging, reading scroll, loading/empty/error/offline states, keyboard navigation and accessibility. No SQL/network in Views or ViewModels.

## Phase 03 — SQLite & Search

Only after local-storage ADR approval, introduce Infrastructure storage. Implement migrations, WAL/foreign keys, repositories, FTS5, safe search grammar and integration tests. Verify Cyrillic and account isolation. Never store secrets in SQLite.

## Phase 04 — MIME & Attachments

Only after transport/MIME dependency approval, process offline `.eml` fixtures with MimeKit. Cover multipart, encodings, inline CID, malformed MIME and bounded parsing. Implement safe attachment saving. HTML remains plain-text fallback until HTML renderer ADR is approved.

## Phase 05 — IMAP Sync

Use a local controlled IMAP test server only. Implement initial/delta sync, UIDVALIDITY/UID/MODSEQ checkpoints, IDLE/polling fallback, reconnect/backoff and offline operation replay. Ensure crash/retry does not duplicate local records. No personal accounts.

## Phase 06 — Accounts & OAuth

Start with fake/local auth flows. After OAuth ADR/provider registration and separate permission, validate controlled test accounts. Implement PKCE, token refresh serialization, invalid_grant re-auth state and approved platform secret stores. Tokens must never enter logs or SQLite.

## Phase 07 — Compose & SMTP

Implement compose, autosave drafts, MIME creation and local SMTP sink tests. Add outbox state machine `queued/sending/sent/failed/unknown`. If transport disconnects after SMTP DATA, mark unknown and do not automatically resend. Real SMTP requires separate approval.

## Phase 08 — Feature Complete

Integrate multi-account daily workflows: unified inbox, folders, flags, archive/trash, conversations, filters, notifications and settings. Prioritize correctness and account isolation over new features.

## Phase 09 — Hardening

Profile startup, search, memory, CPU idle and large-mailbox behavior. Run threat modelling and failure injection. Fix leaks, unbounded queues, UI-thread blocking, sensitive logging and recovery defects. Record measured performance rather than estimates.

## Phase 10 — Platform Integration

Complete Windows/macOS platform services, notifications, secure storage, mailto handling, packaging, signing/notarization preparation and platform smoke tests. A successful build alone is not platform acceptance.

## Phase 11 — Alpha/Beta/RC

Freeze architecture/features progressively, run regression suites, migration/update tests and controlled provider smoke tests only under explicit permission. Triage defects by severity. Prepare release candidate artifacts and release notes.

## Phase 12 — Production Release

Produce signed/notarized release artifacts, validate update/rollback, publish v1.0.0 only after all release gates pass, and establish post-release monitoring/hotfix procedure.

## Standard completion report

For each task, Codex must report: issue IDs, files changed, architectural decisions used, commands executed, build/test results, security/privacy considerations, Windows/macOS validation status, known limitations, and recommended next issue. Do not claim unsupported validation.
