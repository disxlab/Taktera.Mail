# Application API contracts — proposed C# design

**Важно:** иллюстративные контракты для обсуждения, не готовый код проекта. Namespace и типы согласовать после ADR. Domain не содержит зависимостей от MailKit/SQLite/Avalonia. DTO immutable. `CancellationToken` для каждого I/O. Курсоры opaque и имеют стабильный порядок.

```csharp
namespace Taktera.Mail.Application.Contracts;

public readonly record struct AccountId(Guid Value);
public readonly record struct FolderId(Guid Value);
public readonly record struct LocalMessageId(Guid Value);
public readonly record struct DraftId(Guid Value);
public readonly record struct OutboxId(Guid Value);

public enum MailErrorCode
{
    NetworkUnavailable, AuthenticationRequired, PermissionDenied,
    NotFound, Conflict, RateLimited, StorageUnavailable,
    InvalidInput, Unsupported, Unknown
}

public sealed record MailError(MailErrorCode Code, string SafeMessage, bool Retryable);
public sealed record Result<T>(T? Value, MailError? Error)
{
    public bool IsSuccess => Error is null;
}

public sealed record PageRequest(int Limit, string? Cursor);
public sealed record Page<T>(IReadOnlyList<T> Items, string? NextCursor);
public sealed record AccountSummary(AccountId Id, string Email, string DisplayName,
    bool IsPaused, bool RequiresAuthentication);
public sealed record FolderSummary(FolderId Id, AccountId AccountId,
    string DisplayName, string? SpecialUse, int UnreadCount);
public sealed record MessageSummary(LocalMessageId Id, AccountId AccountId,
    FolderId FolderId, string Subject, string FromDisplay,
    DateTimeOffset ReceivedAt, bool IsRead, bool IsStarred, bool HasAttachments);
public sealed record MessageDetail(LocalMessageId Id, string Subject,
    string PlainText, string? SanitizedHtml, IReadOnlyList<AttachmentSummary> Attachments);
public sealed record AttachmentSummary(Guid Id, string SafeFileName, long SizeBytes,
    string MediaType);
public sealed record MailboxQuery(AccountId? AccountId, FolderId? FolderId,
    bool? IsRead, bool? HasAttachments, DateTimeOffset? From, DateTimeOffset? To);
public sealed record SearchQuery(string Text, MailboxQuery Filter);

public interface IAccountService
{
    Task<Result<Page<AccountSummary>>> ListAsync(PageRequest page, CancellationToken ct);
    Task<Result<AccountId>> CreatePendingAsync(string email, CancellationToken ct);
    Task<Result<bool>> DeleteAsync(AccountId accountId, bool deleteLocalCache, CancellationToken ct);
}

public interface IMailboxService
{
    Task<Result<Page<FolderSummary>>> GetFoldersAsync(AccountId accountId, PageRequest page, CancellationToken ct);
    Task<Result<Page<MessageSummary>>> ListMessagesAsync(MailboxQuery filter, PageRequest page, CancellationToken ct);
    Task<Result<MessageDetail>> GetMessageAsync(LocalMessageId messageId, CancellationToken ct);
    Task<Result<bool>> SetReadAsync(LocalMessageId messageId, bool isRead, CancellationToken ct);
    Task<Result<bool>> MoveAsync(LocalMessageId messageId, FolderId destination, CancellationToken ct);
}

public interface ISearchService
{
    Task<Result<Page<MessageSummary>>> SearchAsync(SearchQuery query, PageRequest page, CancellationToken ct);
}

public interface ISynchronizationService
{
    Task<Result<bool>> SyncAccountAsync(AccountId accountId, CancellationToken ct);
    Task<Result<bool>> ReplayPendingOperationsAsync(AccountId accountId, CancellationToken ct);
}

public enum OutboxStatus { Queued, Sending, Sent, Failed, Unknown }
public sealed record ComposeRecipient(string DisplayName, string Address);
public sealed record DraftContent(DraftId Id, AccountId AccountId,
    IReadOnlyList<ComposeRecipient> To, IReadOnlyList<ComposeRecipient> Cc,
    IReadOnlyList<ComposeRecipient> Bcc, string Subject, string BodyPlainText);

public interface IComposeService
{
    Task<Result<DraftId>> SaveDraftAsync(DraftContent draft, CancellationToken ct);
    Task<Result<OutboxId>> QueueSendAsync(DraftId draftId, CancellationToken ct);
    Task<Result<bool>> CancelQueuedAsync(OutboxId outboxId, CancellationToken ct);
}

public interface ICredentialVault
{
    Task StoreAsync(AccountId accountId, string purpose, string secret, CancellationToken ct);
    Task<string?> ReadAsync(AccountId accountId, string purpose, CancellationToken ct);
    Task DeleteAllAsync(AccountId accountId, CancellationToken ct);
}
```

## Contract rules
1. `Result<T>` is proposed; may be replaced with an established discriminated-result approach after review. No leaking raw provider exceptions into UI.
2. Local IDs are immutable; `Message-Id` header not used as unique key.
3. All pagination has bounded `Limit` (suggest 1–200) and deterministic cursor. Cursors must be validated and tied to query/sort.
4. Search query text is user input; FTS grammar compilation belongs to Infrastructure.
5. `SanitizedHtml` is an explicit trust boundary: rendering must still block active/external resources.
6. Credentials interface belongs in Application, implementations platform-specific; `purpose` must be constrained to defined values in final contract.
7. Transport-specific operations and MIME conversion are adapter interfaces; concrete MailKit types never cross boundary.
8. Async cancellation must propagate to network/SQLite operations where supported.
9. Additional contracts (transport, repository, OAuth, notifications) should be introduced per concrete use case, not as empty scaffolding.

## Use-case acceptance examples
- `ListMessagesAsync`: empty page, 100k corpus, deleted cursor target, cancellation, account isolation.
- `MoveAsync`: offline queue, conflicting server move, nonexistent destination, duplicate command.
- `QueueSendAsync`: idempotency key required in final design; after ambiguous SMTP result never automatic retry.
- `DeleteAsync`: explicit choice of cache retention/deletion; secret cleanup is mandatory.
