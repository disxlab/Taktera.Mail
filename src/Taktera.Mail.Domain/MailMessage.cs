namespace Taktera.Mail.Domain;

/// <summary>
/// Lightweight local message metadata. Bodies, attachments and folder/IMAP mappings
/// are deliberately separate and will be added with the persistence design.
/// </summary>
public sealed class MailMessage
{
    public Guid Id { get; }
    public Guid AccountId { get; }
    public string Subject { get; }
    public DateTimeOffset ReceivedAt { get; }
    public bool IsRead { get; private set; }

    public MailMessage(Guid id, Guid accountId, string subject, DateTimeOffset receivedAt, bool isRead = false)
    {
        Id = EntityId.Require(id, nameof(id));
        AccountId = EntityId.Require(accountId, nameof(accountId));
        ArgumentNullException.ThrowIfNull(subject);
        // Empty subjects are valid; do not substitute UI text into stored data.
        Subject = subject;
        ReceivedAt = receivedAt.ToUniversalTime();
        IsRead = isRead;
    }

    public void MarkAsRead() => IsRead = true;

    public void MarkAsUnread() => IsRead = false;
}
