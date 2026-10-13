namespace Taktera.Mail.Application.Synchronization;

/// <summary>Application boundary for future account synchronization; no implementation yet.</summary>
public interface IMailSynchronizer
{
    /// <summary>
    /// Synchronizes one local account. Implementations must reject empty identifiers,
    /// honor cancellation and serialize overlapping work for the same account.
    /// Completion means local changes and the synchronization checkpoint are committed.
    /// </summary>
    Task SynchronizeAsync(Guid accountId, CancellationToken cancellationToken);
}
