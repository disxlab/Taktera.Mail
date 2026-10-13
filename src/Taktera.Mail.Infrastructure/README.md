# Infrastructure

Approved initial project boundary: Infrastructure → Application → Domain.
No transport, storage, authentication packages or runtime services are installed yet.
Desktop does not reference this project until an implementation is needed.

Future adapters for MailKit/MimeKit, SQLite FTS5 and OAuth2 belong here after their
respective design decisions. Add directories when implementations exist; do not add
placeholder services that silently report successful synchronization.
