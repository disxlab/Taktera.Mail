# 0004 — Mail transport
Status: Proposed
Date: 2026-10-13

## Контекст
Интерфейсы синхронизации старой ветки не перенесены. В будущем требуются IMAP/SMTP,
MIME и ограниченное потребление ресурсов.

## Альтернативы
- MailKit + MimeKit: переносимые IMAP/SMTP и MIME без самостоятельного протокола.
- Самописный IMAP/SMTP: высокий риск ошибок протокола и безопасности.
- Только provider HTTP API: меньше охват провайдеров; Graph рассматривается отдельно.

## Рекомендуемое решение
MailKit для транспорта, MimeKit для parse/compose. Ленивая загрузка тел/вложений, потоковое
чтение и ограничение размера/глубины MIME, никаких полных массивов всех писем.
Начальный бюджет: одна IMAP-сессия и SMTP по требованию на аккаунт, общий лимит активных
аккаунтов/операций. Значения уточнить измерениями и server limits; очереди bounded.
Операции одного ImapClient сериализуются; IDLE завершается перед командой.
Нет IDLE — polling с jitter; reconnect с ограниченным exponential backoff и CancellationToken.
TLS проверяется стандартно, автоматического отката к небезопасной аутентификации нет.

Initial sync порциями, checkpoint коммитится вместе с данными. Отдельное сопоставление
account/folder/UIDVALIDITY/UID; reset UIDVALIDITY не удаляет локальные черновики.
Использовать CONDSTORE/QRESYNC при наличии, иначе UID/flags reconciliation.
Remote delete, move, rename и offline-конфликты требуют явной политики.
Ошибки разделять: отмена, сеть/timeout (ограниченный retry), auth/policy (действие пользователя),
malformed MIME (изолировать письмо), storage failure (не продвигать checkpoint).

SMTP: durable очередь, стабильный Message-Id для попытки, но без обещания exactly-once.
Обрыв после DATA дает unknown, без автоматического повтора. Ошибка IMAP APPEND в Sent
не повторяет SMTP; учитывать server-side Sent. Bcc остается в envelope, не в видимых заголовках.

## Риски
Лимиты провайдера, дубликаты при retry, большие вложения, неверный UID mapping,
расхождение Gmail labels и папок. Стабильный Message-Id не гарантирует дедупликацию сервером.

## Последствия
Понадобятся локальные protocol fixtures и state-machine тесты. На этапе 0 пакеты/клиенты не создаются.

## Критерии проверки
На локальном стенде: IDLE interruption, network flap, cancel/shutdown, UIDVALIDITY reset,
повтор батча, deletion/move, throttling, invalid MIME, SMTP unknown и повтор только Sent-save.
Измерить RAM с большим вложением, число соединений и глубину очереди.

## Источники
- [MailKit ImapClient](https://mimekit.net/docs/html/T_MailKit_Net_Imap_ImapClient.htm)
- [IdleAsync](https://mimekit.net/docs/html/M_MailKit_Net_Imap_ImapClient_IdleAsync.htm)
