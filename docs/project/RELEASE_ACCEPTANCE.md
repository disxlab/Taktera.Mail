# Taktera.Mail — Release acceptance gates

A release candidate may proceed to production only when all applicable gates are satisfied.

- Release builds pass on supported Windows and macOS targets.
- Unit/integration/UI smoke tests pass.
- Database migrations are tested on copies of representative prior-version data.
- Update and rollback procedures are verified without user-data loss.
- No unresolved blocker/critical defects; high-severity issues have explicit release disposition.
- Dependency/security audit completed; no silently suppressed NU1903 or equivalent warnings.
- OAuth/secrets/logging review confirms tokens/passwords/mail bodies are not leaked.
- HTML/attachment safety rules verified.
- IMAP sync recovery, UIDVALIDITY reset, reconnect and offline replay scenarios pass.
- SMTP ambiguous-delivery scenario produces `unknown` and never automatic duplicate send.
- Multiple-account isolation and deletion/cleanup behavior pass.
- Search correctness/performance tested on agreed corpus.
- Installer/package signatures are valid; macOS notarization completed when applicable.
- Release notes, known limitations, privacy/security docs and support path are ready.
- Final production release requires explicit maintainer/user approval.
