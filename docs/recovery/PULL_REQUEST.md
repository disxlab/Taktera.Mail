# Restore the M00 Avalonia demo and engineering baseline without architecture implementations

The current main branch contains planning documents but no buildable application. This change
selectively restores the existing demo from 5ac8ae11813f9cf07763fc994a0e1f79a11f3594,
alongside main's unchanged docs/project documentation.

Includes the Desktop UI, empty Application/Domain projects, SDK/build settings, Windows/macOS
CI, full development rules adapted to the current scope, and six Proposed ADRs. Infrastructure,
domain models, synchronization contracts and their tests are deliberately excluded under the
authorized recovery scope. No architecture ADR is approved by this change.

The existing Tmds.DBus.Protocol 0.21.3 security override remains. NuGet Audit is enabled with
audit warnings treated as errors. Three offline UI tests verify synthetic message selection,
reading-pane updates and long-message scrolling at minimum/default window sizes.

Validation on Windows, SDK 10.0.401:
- Restore succeeded; Release build: 0 errors, 0 warnings across 4 projects.
- 3 UI tests passed; 0 failures/skips.
- NuGet audit including transitives: no known vulnerable packages in the configured source.
- Native application window opened and process responded.
- Existing docs/project files are unchanged; bin/obj and user ZIP files are excluded.

macOS runtime/UI and remote CI remain unverified until this branch is published. This is an
M00 recovery, not M00 acceptance or M01 implementation. No real mail accounts, OAuth or sending.
See docs/recovery/M00_SAFE_SOURCE_RECOVERY.md for provenance, commands and limitations.

Related: #3, #4, #5, #6, #7, #8, #9. Do not automatically close these issues; outstanding
acceptance criteria remain. Draft PR; no merge without explicit approval.
