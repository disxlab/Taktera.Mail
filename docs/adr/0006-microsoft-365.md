# 0006 — Microsoft 365 transport
Status: Proposed
Date: 2026-10-13

## Контекст
Microsoft 365 может запрещать IMAP или SMTP AUTH независимо от успешного OAuth-входа.
Нужно учитывать администраторские политики, MFA, Conditional Access и delegated consent.

## Альтернативы
- IMAP/SMTP + OAuth: общий MailKit-код и переносимость, но не покрывает все tenant policies.
- Microsoft Graph: HTTPS mail API и delta, иные scopes/идентификаторы, throttling и отдельный адаптер.
- Автоматическое переключение транспорта: удобно внешне, но риск дублей и непредсказуемого consent.
- Требовать ослабления tenant security: неприемлемый продуктовый путь.

## Рекомендуемое решение
Первый транспорт — IMAP/SMTP OAuth там, где tenant и mailbox разрешают его.
Показывать точную причину policy/auth failure и доступный следующий шаг; не советовать
отключать MFA/защиту. Не гарантировать доступность SMTP AUTH для любого Microsoft 365 аккаунта.
Graph — отдельный будущий адаптер после утверждения объема корпоративной поддержки.
Не получать Graph scopes «на всякий случай» и не использовать Graph token в IMAP.

При выборе Graph: delegated Mail.Read или Mail.ReadWrite по фактическим операциям,
Mail.Send отдельно; admin consent/Conditional Access могут требовать вмешательства администратора.
Delta/checkpoint и remote IDs специфичны для адаптера; поддержать pagination,
Retry-After и повтор initial sync при недействительном delta token.
Не смешивать одновременно IMAP и Graph checkpoints для одного аккаунта.
Переключение транспорта — явная миграция mapping/cache без повторной отправки Outbox.

## Риски
Graph не обходит Conditional Access и не гарантирует согласие администратора.
Дополнительный адаптер удваивает часть sync-тестов, меняет folder/message semantics.
Принятие HTTP send-запроса не равно подтвержденной доставке получателю.

## Последствия
MVP явно документирует ограничение корпоративных аккаунтов. Поддержка shared mailboxes,
application permissions и фонового daemon-доступа вне объема первого клиента.
На этапе 0 Graph SDK/регистрации/сетевые операции отсутствуют.

## Критерии проверки
После отдельного разрешения: test tenant с разрешенным и запрещенным IMAP/SMTP,
отказ consent и Conditional Access, истекший token, Graph throttling/delta reset.
На offline fixtures проверить policy error UX и отсутствие автоматического повторного send.
Нужны отдельные критерии приемки для каждого заявленного типа аккаунта.

## Источники
- [OAuth для IMAP/SMTP](https://learn.microsoft.com/en-us/exchange/client-developer/legacy-protocols/how-to-authenticate-an-imap-pop-smtp-application-by-using-oauth)
- [SMTP AUTH policies](https://learn.microsoft.com/en-us/exchange/clients-and-mobile-in-exchange-online/authenticated-client-smtp-submission)
- [Graph message delta](https://learn.microsoft.com/en-us/graph/delta-query-messages)
