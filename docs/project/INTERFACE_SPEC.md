# Taktera.Mail — Interface specification

## Goal

Modern desktop email UX for Windows first and macOS second, optimized for speed, clarity, keyboard operation and large mailboxes.

## Primary layout

Three-pane default layout:

1. Navigation/sidebar: accounts, unified inbox, folders, compose button.
2. Message list: paged/virtualized list with selection and quick actions.
3. Reading pane: message header, content, attachments and reply actions.

Support two-pane mode and hidden reading pane. Persist pane widths and user layout.

## Navigation

- Multiple accounts and Unified Inbox.
- Inbox, Starred, Sent, Drafts, Archive, Trash/Junk where available.
- Nested folders and provider-specific labels where applicable.
- Account-level sync/error indicators.
- Search accessible from primary window.

## Message list

- Virtualized/paged; never materialize the whole mailbox in an `ObservableCollection`.
- Unread, starred, attachment and account indicators.
- Sender, subject/preview and localized date/time.
- Multiple selection and batch actions.
- Stable keyboard navigation.
- Empty/loading/offline/error states.
- Context menu and quick actions without blocking UI thread.

## Reading pane

- Subject, sender, recipients, timestamps.
- Plain-text first; safe HTML after renderer ADR approval.
- Scroll long messages.
- Attachments with safe save actions.
- Reply, Reply all, Forward.
- Remote content blocked by default with explicit user action where supported.
- Clear warning states for suspicious/unsupported content.

## Compose

- From identity/account selector.
- To/Cc/Bcc chips with validation.
- Subject, plain-text editor and later approved HTML formatting.
- Drag/drop and picker attachments.
- Signature support.
- Autosave local draft.
- Visible send/queued/sending/failed/unknown state.
- Warn for missing subject and likely forgotten attachment.

## Account setup

- Email-first provider detection.
- OAuth flow where supported, manual IMAP/SMTP configuration as applicable.
- Connection/status progress and actionable errors.
- No secret display after storage.

## Search

- Fast full-text field.
- Filters: account, folder, sender/recipient, date range, unread, starred, attachments.
- Stable paginated results.
- Keyboard focus and Escape behavior defined.

## Settings

- General/theme/language.
- Accounts and identities.
- Signatures.
- Notifications and quiet hours.
- Sync/cache policy.
- Privacy/security.
- Keyboard shortcuts.
- Diagnostics/about/update channel.

## Window behavior

- Minimum window size must remain usable with no clipped critical controls.
- Panels resizable and collapsible.
- Restore previous size/position safely.
- Support high DPI and multiple monitors.
- Native-feeling shortcuts: Ctrl on Windows, Command on macOS where appropriate.

## Accessibility

- Logical tab order.
- Visible focus states.
- Screen-reader names/roles for actionable controls.
- No state communicated by color alone.
- Adequate contrast and target sizes.
- Keyboard access to all core actions.

## Required UI states

Every main surface must specify and render: Loading, Empty, Success, Offline, Error, AuthenticationRequired and Cancelled/Interrupted where applicable.

## Acceptance

UI work is complete only when Release build passes, large fixture lists stay responsive, keyboard navigation works, long messages scroll, minimum window layout remains usable, and Windows/macOS differences are documented and tested at the applicable stage.
