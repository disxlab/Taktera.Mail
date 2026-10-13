namespace Taktera.Mail.Domain;

/// <summary>A local folder identity, independent of its server path or display name.</summary>
public sealed class MailFolder
{
    public Guid Id { get; }
    public Guid AccountId { get; }
    public string Name { get; }

    public MailFolder(Guid id, Guid accountId, string name)
    {
        Id = EntityId.Require(id, nameof(id));
        AccountId = EntityId.Require(accountId, nameof(accountId));
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        // Spaces can be significant in server folder names.
        Name = name;
    }
}
