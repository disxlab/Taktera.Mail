# M00 — Ограниченное восстановление исходников

Дата: 2026-10-13. Ветка: `codex/m00-safe-source-recovery`.
База: `origin/main` / `0428f539175dc1d953fff1eb9859d935aa5b76d3`.
Источник: `5ac8ae11813f9cf07763fc994a0e1f79a11f3594` (`codex/engineering-baseline`).
Аудит сохранен отдельным коммитом `1ecca84` в `codex/tm-00-01-baseline-audit`.

## Разрешенный объем и происхождение

Восстановлен только существующий Desktop-прототип, без написания нового приложения.
Git-история исходных веток сохранена; выбранные файлы получены через
`git restore --source=5ac8ae11813f9cf07763fc994a0e1f79a11f3594 --worktree -- <явный список путей>`.
Коммит источника не становится предком восстановительной ветки: происхождение записано
здесь и в commit message. Merge, rebase и удаление старых веток не выполнялись.

| Файлы из источника | Изменения при восстановлении |
| --- | --- |
| `src/Taktera.Mail.Desktop/Program.cs`, `App.axaml`, `App.axaml.cs`, `MainWindow.axaml`, `MainWindow.axaml.cs`, `Models/MailItem.cs`, `ViewModels/MainWindowViewModel.cs`, `Taktera.Mail.Desktop.csproj` | Без изменений |
| `src/Taktera.Mail.Application/Taktera.Mail.Application.csproj`, `src/Taktera.Mail.Domain/Taktera.Mail.Domain.csproj` | Без изменений; только пустые проекты, ни одного бизнес-класса |
| `global.json`, `Directory.Build.props`, `.github/workflows/ci.yml` | Без изменений |
| `.gitignore` | Добавлено `*.zip`; bin/obj уже игнорируются |
| `Taktera.Mail.slnx` | Исключены Infrastructure и Domain.Tests; осталось 4 проекта |
| `AGENTS.md` | Сохранены все разделы и ограничения; обновлены текущий состав, ссылки и запрет M01 до приемки M00 |
| `tests/Taktera.Mail.Desktop.Tests/Taktera.Mail.Desktop.Tests.csproj` | Без изменений |
| `tests/Taktera.Mail.Desktop.Tests/LayoutTests.cs` | Сохранены 2 layout-теста; добавлен тест выбора второго синтетического письма через ListBox и обновления панели чтения |
| `docs/adr/0001-layered-architecture.md`, `0004-mail-transport.md`, `README.md` | Исправлены описания исходного состояния: Infrastructure/контракты отсутствуют, старый accepted ADR не перенесен |
| `docs/adr/0002-local-storage.md`, `0003-authentication.md`, `0005-html-rendering.md`, `0006-microsoft-365.md` | Без изменений; исключительно Proposed |

Новые файлы текущей работы: этот отчет и `docs/recovery/PULL_REQUEST.md`.
Вся существующая документация `docs/project/` сохранена побайтно относительно базы main.
Корневой README также не заменен старой версией.

## Сохранение правил и архитектурных границ

Сравнены AGENTS из 505e3c2 и 5ac8ae1, а также действующие docs/project/README,
CODEX_PHASE_PROMPTS и ограничения пользователя. В main AGENTS отсутствовал.
За основу взят полный документ 5ac8ae1, а не сокращенное изложение.
Сохранены правила Git/согласований, MVVM, async/cancellation, секретов/TLS/HTML/SQL,
миграций, проверки UI, тестов и запрет подавления предупреждений.
Описание 6 проектов заменено фактическими 4; ссылки направлены в текущий docs/project.

Разрешены только пустые shell-проекты Desktop → Application → Domain.
Не перенесены Domain/EntityId.cs, MailAccount.cs, MailFolder.cs, MailMessage.cs,
Application/Synchronization, весь Infrastructure, Domain.Tests и исторический accepted ADR-001.
Старые bin/obj этих проектов могут оставаться на диске как сохраненные артефакты;
они не включены в solution и Git. Ни одна implementation-служба/адаптер не восстановлена.
Все шесть ADR имеют `Status: Proposed`; разрешение на восстановление не утверждает их.

## Проверки

Windows x64; SDK **10.0.401**, target **net10.0**, Avalonia **11.3.6**,
CommunityToolkit.Mvvm **8.4.0**, Tmds.DBus.Protocol **0.21.3**.
Тесты: Avalonia.Headless.XUnit 11.3.6, xUnit 2.9.3, Test SDK 17.14.1, VS runner 3.1.4.
В PowerShell использован абсолютный `C:\Program Files\dotnet\dotnet.exe`.

| Выполненная команда | Результат |
| --- | --- |
| `dotnet restore Taktera.Mail.slnx` | Exit 0 |
| `dotnet build Taktera.Mail.slnx --configuration Release --no-restore` | Exit 0; 4 проекта, 0 ошибок, 0 предупреждений |
| `dotnet test Taktera.Mail.slnx --configuration Release --no-build --no-restore` | Exit 0; 3 passed, 0 failed/skipped |
| `dotnet list Taktera.Mail.slnx package --include-transitive --vulnerable` | Exit 0; известных уязвимостей нет по api.nuget.org во всех 4 проектах |
| `dotnet nuget why src/Taktera.Mail.Desktop/Taktera.Mail.Desktop.csproj Tmds.DBus.Protocol` | Прямая ссылка и Avalonia.Desktop → X11 → FreeDesktop разрешают 0.21.3 |
| `dotnet src/Taktera.Mail.Desktop/bin/Release/net10.0/Taktera.Mail.Desktop.dll` | Процесс продолжает работу, окно Taktera.Mail открыто |
| `Get-Process dotnet ... MainWindowTitle/Responding` | Новый процесс 22964, MainWindowTitle=Taktera.Mail, Responding=True |
| `git diff origin/main -- docs/project` | Изменений нет |

NuGet Audit остается true/all/low. NU1900–NU1905 не подавлены, считаются ошибками.
Тест выбора проверяет 3 элемента, начальный выбор и двустороннюю привязку ListBox → ViewModel →
тема/тело чтения. Layout-тесты проверяют scroll до конца длинного письма при 850×520 и 1220×780.
Нативный запуск подтверждает открытие окна; интерактивные сценарии проверены headless,
не заявляются как полный ручной smoke на Windows/macOS.

## Сохранность файлов и ограничения

Пользовательский ZIP не изменен: SHA256
`4CCCFDBB55D755854F082863DEB171A701A706DB21B4E60EDA482115DEDCA4C6`.
bin/obj и ZIP не добавляются в Git; при staging используется явный список.
Аудит остается в отдельной ветке/коммите; пользовательские файлы не удалялись.

CI восстановлен для Windows/macOS с contents:read, без секретов; удаленные jobs пока не
запускались, поскольку публикация ожидает разрешения. macOS native UI не проверен.
Empty state/DPI/клавиатура, .editorconfig и итоговая приемка M00 остаются отдельными задачами.
Приложение остается демонстрацией; кнопки почтовых операций не реализованы.
Реальных аккаунтов, OAuth, отправки, SQLite, DI и M01 нет.

Связанные задачи: TM-00-01 восстановление базы; заготовки TM-00-02/03/04/05/06/07
переиспользованы, но их полная приемка и закрытие Issues не заявляются.
Следующее действие после разрешения публикации — Draft PR в main и проверка CI;
затем рассмотрение Proposed ADR и оставшихся критериев M00. Merge запрещен без отдельного разрешения.
