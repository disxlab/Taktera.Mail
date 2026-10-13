# Taktera.Mail — Design System

## Principles

- Calm, focused, dense enough for professional mail without visual clutter.
- Fast perceived response: immediate selection feedback, skeletons only where useful, no decorative animation that delays work.
- Cross-platform consistency with platform-aware shortcuts and window chrome.
- Accessibility is a baseline requirement.

## Foundations

Use semantic design tokens rather than hard-coded visual values in Views. Define tokens for background/surface/elevated surface, primary/secondary text, accent, border/divider, success/warning/error/info, selected/hover/focus states.

Spacing should follow a compact 4/8-based scale. Components should use shared corner-radius, border and elevation tokens. Typography must distinguish application title, section title, message subject, body, metadata and caption.

## Themes

- Light
- Dark
- System-following

All semantic colors require equivalent theme values. Do not encode meaning only through hue.

## Component library

Create reusable styles/templates for:

- Buttons: primary, secondary, ghost, icon, destructive.
- Text fields/search fields.
- Account and folder navigation items.
- Message list rows.
- Recipient chips.
- Badges/counters/status pills.
- Tabs/segmented controls where needed.
- Context menus and flyouts.
- Dialogs and confirmations.
- Toast/inline notifications.
- Progress and synchronization indicators.
- Attachment cards.
- Empty/error/offline states.

## Icons

Lucide is the preferred icon vocabulary for the proposed system. Keep icons optically consistent, use a shared size set, and pair ambiguous icons with accessible labels/tooltips. Do not use decorative icons where text is clearer.

## Interaction states

Every interactive component must define Normal, Hover, Pressed, Focused, Selected, Disabled, Loading and Error where applicable.

## Motion

Motion is subtle and functional. Respect reduced-motion preferences. Avoid long transitions during mailbox navigation or message selection.

## Density and responsiveness

Desktop-first. Support compact professional information density while maintaining readable touch/click targets. Panels must collapse gracefully at minimum window widths.

## Implementation

Avalonia styles/resources should be centralized. Views consume semantic resources; ViewModels must not contain visual constants. Avoid one-off styling unless justified and documented.
