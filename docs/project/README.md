# Taktera.Mail — Project Documentation

This directory contains the project planning baseline for ChatGPT, Codex and maintainers. It is a planning reference, not approval of proposed ADRs or authorization to implement all stages.

Read in this order:

1. `../../AGENTS.md` — repository rules and constraints.
2. `ROADMAP.md` — 13-stage delivery plan to v1.0.
3. `SPECIFICATION.md` — technical specification.
4. `API_CONTRACTS.md` — application/API contracts.
5. `DATABASE_DESIGN.md` — local data model proposal (executable SQL schema has not yet been imported).
6. `INTERFACE_SPEC.md` and `DESIGN_SYSTEM.md` — UX/UI requirements.
7. `ADR_DECISIONS.md` — architecture decisions that still require approval.
8. `CODEX_PHASE_PROMPTS.md` — sequential Codex task prompts.
9. GitHub Issues `TM-00-*` through `TM-12-*` — backlog and acceptance criteria (the consolidated `ISSUES_FULL.md` has not yet been imported).
10. `RELEASE_ACCEPTANCE.md` — release gate checklist.

## Governance

- This documentation does not override `AGENTS.md`.
- Architecture-changing decisions remain proposals until explicitly approved through ADR.
- Do not connect real mail accounts or perform real OAuth/mail sending without separate approval.
- Do not merge into `main` without user authorization.
- Prefer one focused branch/PR per issue or tightly related issue group.

## Development start

Codex should begin with Stage 00 / Engineering Baseline only. It should inspect the repository, read all governance and project docs, map existing GitHub issues `TM-00-*`, and implement only the approved non-architectural baseline work. Where an issue requires an ADR decision, prepare or update the ADR and stop before implementation until approval is given.
