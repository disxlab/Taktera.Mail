# Taktera.Mail — Architecture decisions to approve

The following decisions remain **Proposed** until explicitly approved. Do not treat this file as authorization to implement them.

1. Layering: keep Desktop → Application → Domain; add Infrastructure as outer adapters; Desktop acts as composition root.
2. Local storage: Microsoft.Data.Sqlite candidate; FTS5; migration strategy; WAL; cache placement; encryption policy.
3. Authentication: OAuth2 Authorization Code + PKCE; provider SDK choices; redirect URI/scopes; Windows Credential Manager/DPAPI adapter; macOS Keychain.
4. Mail transport: MailKit IMAP/SMTP and MimeKit; connection serialization; bounded concurrency; retry/backoff policy.
5. HTML rendering: choose a safe cross-platform renderer; scripts and remote resources disabled by default; plain text remains fallback until approved.
6. Microsoft 365: decide behavior when tenant policy disables IMAP/SMTP; optionally add a separate Graph adapter later.
7. Local cache retention: quotas, cleanup and account deletion policy.
8. Update/signing pipeline: Windows signing, macOS signing/notarization, rollback and authenticated updates.
9. Diagnostics/privacy: safe logs with secret/content redaction and explicit diagnostic export.
10. Packaging/release channels: supported architectures, installers and beta/stable channels.

Each ADR must record context, alternatives, decision, consequences, risks and verification criteria.
