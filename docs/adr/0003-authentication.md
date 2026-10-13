# 0003 — Authentication
Status: Proposed
Date: 2026-10-13

## Контекст
Авторизации нет. Desktop public client не способен хранить client secret.
Нужны Google/Microsoft и безопасное восстановление сессии на обеих ОС.

## Альтернативы
- Password/basic authentication: несовместима с целевым современным OAuth-сценарием.
- Embedded browser/самописный OAuth: сложнее безопасность и совместимость политик.
- Системный браузер + provider SDK: рекомендуемый путь.
- Device code: отдельный UX и ограничения политик; не основной поток.

## Рекомендуемое решение
Authorization Code + PKCE S256 в системном браузере. Проверять state, redirect URI,
время жизни одноразового verifier; loopback listener только на локальном интерфейсе и с timeout.
Для Microsoft рассмотреть MSAL, для Google — поддерживаемый native OAuth SDK.
Настройки provider/client ID/scopes не секреты; не встраивать client secret.
Запрашивать минимальные scopes для выбранного транспорта: Graph и IMAP токены не взаимозаменяемы.
Google IMAP scope и consent verification согласовать до выпуска.

Windows Credential Manager и macOS Keychain через интерфейс секретов.
Не писать токены в SQLite, диагностические логи или Git. Учитывать лимит размера записи
Credential Manager: не обрезать сериализованный token cache; если не помещается, отдельный
ADR для DPAPI-защищенного cache-файла. На macOS обрабатывать locked/denied Keychain.
Один refresh на аккаунт; атомарная замена ротированного refresh token; invalid_grant
останавливает повторы и требует входа. Logout удаляет credentials и применяет политику кеша писем.

## Риски
Отозванное согласие, MFA/Conditional Access, отказ Keychain, несовпадение audience,
кража токена процессом того же пользователя. OS vault не защищает от полностью скомпрометированной сессии.

## Последствия
Нужны регистрации приложений и проверка политик провайдеров; реализация и реальные входы
не входят в этап 0. Сначала fake token endpoint/clock и тесты lifecycle.

## Критерии проверки
PKCE/state mismatch, отмена/timeout, replay callback, одновременный refresh, ротация и отзыв.
Проверить отсутствие секретов в SQLite/логах, отказ OS vault и cleanup аккаунта.
Реальные тестовые аккаунты только после отдельного разрешения.

## Источники
- [Google desktop OAuth](https://developers.google.com/identity/protocols/oauth2/native-app)
- [Microsoft authorization code flow](https://learn.microsoft.com/en-us/entra/identity-platform/v2-oauth2-auth-code-flow)
- [Windows Credential Manager](https://learn.microsoft.com/en-us/windows/win32/secauthn/credentials-management)
- [Apple Keychain](https://developer.apple.com/documentation/security/keychain-services)
