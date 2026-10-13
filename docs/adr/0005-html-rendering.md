# 0005 — Safe HTML rendering
Status: Proposed
Date: 2026-10-13

## Контекст
Сейчас письмо отображается TextBlock как plain text. HTML из почты недоверенный;
важны Windows/macOS, RAM, размер дистрибутива и стоимость сопровождения.

## Альтернативы
| Вариант | Совместимость и затраты | Ограничения |
| --- | --- | --- |
| Plain text / ограниченное отображение средствами Avalonia | Минимум зависимостей и памяти | Нет полноценной верстки HTML-писем |
| NativeWebView: WebView2 / WKWebView | Windows/macOS, системный движок без bundled Chromium | Native interop, лицензирование компонента, разные движки и API защиты |
| Bundled Chromium (CEF/коммерческий wrapper) | Более одинаковая верстка | Размер поставки, несколько процессов, обновления безопасности и лицензия |
| Самописный HTML renderer | Контроль набора тегов | Высокая стоимость, слабая совместимость CSS, риск sanitizer |

## Рекомендуемое решение
Сохранить plain text до отдельного security/performance spike.
Предпочтительный кандидат для проверки — системные WebView2 (Windows) и WKWebView (macOS)
через поддерживаемый Avalonia-адаптер. NativeWebView — коммерческий компонент;
лицензию, совместимость с текущим Avalonia 11.3.6 и нужные API проверить до выбора.
Не считать наличие cross-platform API доказательством одинаковой изоляции на обеих ОС.

Defense in depth: allowlist sanitizer и строгая CSP, отключение JS/bridge, блокировка forms,
navigation, downloads, popups и всех сетевых подзапросов по умолчанию.
Удалять CSS url/import, tracking pixels, опасные схемы/file URLs; не доверять base URI.
CID-вложения выдавать через ограниченный локальный resolver без произвольного доступа к диску.
Внешние ссылки открывать только по явному действию с проверкой схемы.
Использовать временный изолированный browser profile без общей сессии/cookies.
Загрузка внешних изображений — явное разрешение пользователя для письма.

## Риски
Sanitizer не равен sandbox; движок может отправлять запросы до навигационного события.
Не все interception API равнозначны на macOS/Windows. Системный runtime также потребляет RAM,
а его отсутствие/обновление на Windows требует политики установки.
Нельзя обещать конкретные мегабайты без измерений.

## Последствия
HTML остается отложенным. Если нельзя блокировать все подресурсы и bridge на обеих ОС,
использовать plain text, а не ослаблять защиту. Не добавлять пакет на этапе 0.

## Критерии проверки
На обеих ОС корпус malicious HTML/CSS/SVG/CID: 0 неожиданных DNS/HTTP/file запросов,
0 исполнения скриптов, защита от navigation/download и escape локального resolver.
Сравнить cold/warm start, working set всего process tree, idle CPU и размер установщика
для plain text/native/bundled вариантов на одинаковом корпусе. Проверить DPI/keyboard/a11y.
Результат spike и лицензия требуют явного утверждения перед реализацией.

## Источники
- [Avalonia NativeWebView](https://docs.avaloniaui.net/controls/web/nativewebview)
- [Avalonia embedding web content](https://docs.avaloniaui.net/docs/app-development/embedding-web-content)
- [Avalonia о системном движке и коммерческом компоненте](https://avaloniaui.net/electron-compare)
