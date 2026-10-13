using Taktera.Mail.Domain;

namespace Taktera.Mail.Application.Synchronization;

/// <summary>Boundary for incremental synchronization of a known local folder.</summary>
public interface IMailFolderSynchronizer
{
    /// <summary>
    /// Synchronizes metadata in bounded batches, without eagerly loading bodies or attachments.
    /// Implementations must honor cancellation, validate account ownership and atomically
    /// commit each batch with its checkpoint. Repeating a batch must not create duplicates.
    /// </summary>
    Task SynchronizeAsync(MailFolder folder, CancellationToken cancellationToken);
}
