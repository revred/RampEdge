# WP12 — Internationalization (i18n)

**Epic:** i18n & Currency | **Priority:** 3 | **Complexity:** Medium

## Intent
Serve UI strings and number/date/currency formats per locale; allow language toggle and persistence.

## Scope
- ≥2 language packs; auto‑detect; RTL readiness; currency formatting cooperation

## Stories & AC
- **Visitor:** Switch language → **AC:** All visible strings swap; preference persisted.
- **Owner:** Add translations easily → **AC:** 98% key coverage check.

## Implementation
- Resource files (.resx for Blazor); formatter utilities; locale middleware
- Ensure currency/tax labels sync with WP04

## Test Plan
- Snapshot tests per locale; manual RTL smoke

## DoD
- [ ] Packs shipped (e.g., en, fr); toggle present; persisted
- [ ] 98%+ keys coverage; RTL validated
