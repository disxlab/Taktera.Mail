# ADR-001: Minimal mail foundation

Status: accepted for the scope explicitly requested by the user on 2026-10-13.

## Decision

Keep Desktop → Application → Domain and add an empty Infrastructure → Application project.
Use the transitive Domain reference; avoid redundant references and new runtime packages.
Add a solution to build all projects, and one xUnit domain test project.
Do not connect Infrastructure to Desktop until an implementation is available.

Domain models use non-empty local Guid IDs. Accounts contain metadata, not credentials;
address setup/provider validation remains future work. Folder names preserve significant spaces.
Messages contain only metadata, normalize dates to UTC and support idempotent read/unread changes.
Bodies and attachment arrays are absent to avoid eager payload allocation.
Referential integrity across accounts/folders/messages remains a future repository responsibility.

Application exposes account and folder synchronization contracts returning Task with explicit
CancellationToken. They describe future serialization, bounded batches and checkpoint semantics,
but provide no fake success implementation. Transport-specific UID/UIDVALIDITY mappings and
the persistence schema are deferred. No new service runs at startup.

## Alternatives and limits

A DI framework, generic repositories, provider SDKs and strongly typed wrapper IDs are deferred
until concrete scenarios need them. xUnit/Test SDK dependencies exist only in the test project.
No MailKit, MimeKit, SQLite, OAuth or HTML renderer dependency is introduced here.

The Tmds.DBus.Protocol 0.21.3 direct reference is a narrow security override of the existing
Avalonia transitive dependency. The maintainer backported the fix to this release; upgrading
the full UI stack or switching the protocol library to 0.92 would have a larger compatibility
surface. Remove the override when an approved Avalonia update resolves a patched version itself.

Validation: Release solution build, domain tests and transitive NuGet vulnerability audit.
Windows/macOS UI runtime behavior and future synchronization semantics need their own tests.
