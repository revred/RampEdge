# WP10 — Filtering & Facets

**Epic:** Search & Navigation | **Priority:** 4 | **Complexity:** Medium

## Intent
Attribute‑based AND filters for category/spec/material/tolerance with clear reset.

## Scope
- Facet components; query‑string sync; empty‑state UX

## Stories & AC
- **Visitor:** Apply multiple filters → **AC:** Only matching products shown; clear‑all resets.
- **Developer:** Place `<Filters/>` and wire to list → **AC:** Works with zero custom code.

## Implementation
- Attribute bitsets or server‑side filter API (large catalogs)
- Persisted state in URL; SSR pre‑filter when possible

## Test Plan
- E2E: multi‑filter combos; browser back/forward

## DoD
- [ ] Facets render; AND logic correct; clear‑all present
- [ ] SSR-friendly first render
