using Taktera.Mail.Domain;
using Xunit;

namespace Taktera.Mail.Domain.Tests;

public sealed class MailModelTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" \t")]
    public void AccountRequiresAddress(string? address)
    {
        Assert.ThrowsAny<ArgumentException>(() => new MailAccount(Guid.NewGuid(), address!, "Demo"));
    }

    [Fact]
    public void AccountTrimsMetadataWithoutChangingAddressCase()
    {
        var id = Guid.NewGuid();
        var account = new MailAccount(id, " User@example.test ", " Demo ");
        Assert.Equal(id, account.Id);
        Assert.Equal("User@example.test", account.Address);
        Assert.Equal("Demo", account.DisplayName);
    }

    [Fact]
    public void AccountAllowsEmptyDisplayNameButRejectsNull()
    {
        Assert.Equal("", new MailAccount(Guid.NewGuid(), "user@example.test", "").DisplayName);
        Assert.Throws<ArgumentNullException>(() => new MailAccount(Guid.NewGuid(), "user@example.test", null!));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void FolderRequiresName(string? name)
    {
        Assert.ThrowsAny<ArgumentException>(() => new MailFolder(Guid.NewGuid(), Guid.NewGuid(), name!));
    }

    [Fact]
    public void SameFolderNameDoesNotCollapseAccountOrFolderIdentity()
    {
        var first = new MailFolder(Guid.NewGuid(), Guid.NewGuid(), " Inbox ");
        var second = new MailFolder(Guid.NewGuid(), Guid.NewGuid(), " Inbox ");
        Assert.Equal(" Inbox ", first.Name);
        Assert.NotEqual(first.Id, second.Id);
        Assert.NotEqual(first.AccountId, second.AccountId);
    }

    [Theory]
    [InlineData("account", "id")]
    [InlineData("folder", "id")]
    [InlineData("folder", "accountId")]
    [InlineData("message", "id")]
    [InlineData("message", "accountId")]
    public void EmptyLocalIdentifiersAreRejected(string model, string parameter)
    {
        var id = parameter == "id" ? Guid.Empty : Guid.NewGuid();
        var accountId = parameter == "accountId" ? Guid.Empty : Guid.NewGuid();
        var error = Assert.Throws<ArgumentException>(() =>
        {
            _ = model switch
            {
                "account" => (object)new MailAccount(id, "user@example.test", ""),
                "folder" => new MailFolder(id, accountId, "Inbox"),
                _ => new MailMessage(id, accountId, "", DateTimeOffset.UnixEpoch)
            };
        });
        Assert.Equal(parameter, error.ParamName);
    }

    [Fact]
    public void MessagePreservesInstantAndNormalizesToUtc()
    {
        var received = new DateTimeOffset(2026, 10, 13, 12, 30, 0, TimeSpan.FromHours(10));
        var message = new MailMessage(Guid.NewGuid(), Guid.NewGuid(), "", received);
        Assert.Equal(received, message.ReceivedAt);
        Assert.Equal(TimeSpan.Zero, message.ReceivedAt.Offset);
        Assert.Equal("", message.Subject);
    }

    [Fact]
    public void MessageRejectsNullSubject()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new MailMessage(Guid.NewGuid(), Guid.NewGuid(), null!, DateTimeOffset.UnixEpoch));
    }

    [Fact]
    public void ReadTransitionsAreIdempotentAndPreserveMetadata()
    {
        var id = Guid.NewGuid();
        var accountId = Guid.NewGuid();
        var message = new MailMessage(id, accountId, " Subject ", DateTimeOffset.UnixEpoch);
        Assert.False(message.IsRead);
        message.MarkAsRead();
        message.MarkAsRead();
        Assert.True(message.IsRead);
        message.MarkAsUnread();
        message.MarkAsUnread();
        Assert.False(message.IsRead);
        Assert.Equal(id, message.Id);
        Assert.Equal(accountId, message.AccountId);
        Assert.Equal(" Subject ", message.Subject);
        Assert.Equal(DateTimeOffset.UnixEpoch, message.ReceivedAt);
    }

    [Fact]
    public void StoredReadStateCanBeRestored()
    {
        var message = new MailMessage(Guid.NewGuid(), Guid.NewGuid(), "", DateTimeOffset.UnixEpoch, isRead: true);
        Assert.True(message.IsRead);
    }
}
