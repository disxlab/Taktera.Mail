namespace Taktera.Mail.Domain;

/// <summary>Local account metadata. Does not contain credentials or server settings.</summary>
public sealed class MailAccount
{
    public Guid Id { get; }
    public string Address { get; }
    public string DisplayName { get; }

    public MailAccount(Guid id, string address, string displayName)
    {
        Id = EntityId.Require(id, nameof(id));
        ArgumentException.ThrowIfNullOrWhiteSpace(address);
        ArgumentNullException.ThrowIfNull(displayName);
        // Preserve case; provider-specific address validation belongs at account setup.
        Address = address.Trim();
        DisplayName = displayName.Trim();
    }
}
