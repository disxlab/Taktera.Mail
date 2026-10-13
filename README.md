# Taktera.Mail

Lightweight Windows-first email client for Windows and macOS.

## Stack
- C# / .NET 10
- Avalonia UI 11
- CommunityToolkit.Mvvm
- Planned: MailKit, MimeKit, SQLite FTS5, OAuth2; Microsoft Graph remains an optional future adapter

## Run
```bash
dotnet run --project src/Taktera.Mail.Desktop
```

## Status
Version 0.1.0: demo UI prototype, no real mail connection or sending yet.

Initial domain models, synchronization contracts and an empty Infrastructure project are
available. The demo UI still uses its own synthetic data. No mail/storage/OAuth adapters exist yet.

## Validate
```bash
dotnet build Taktera.Mail.slnx --configuration Release
dotnet test Taktera.Mail.slnx --configuration Release --no-build
dotnet list Taktera.Mail.slnx package --include-transitive --vulnerable
```

See [development rules](AGENTS.md), [architecture](docs/ARCHITECTURE.md),
[audit](docs/AUDIT.md) and [implementation plan](docs/IMPLEMENTATION_PLAN.md).
