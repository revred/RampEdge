# WP09 — Search & Typeahead

**Epic:** Search & Navigation | **Priority:** 4 | **Complexity:** Medium

## Intent
Near‑instant search within catalog with top‑N suggestions and relevance tuned for specs.

## Scope
- Client prefix index + attribute keys; debounce 120ms; URL state

## Stories & AC
- **Visitor:** “Results update as I type.” → **AC:** <200ms latency for suggestions.
- **Owner:** “Spec search works.” → **AC:** diameter/material fields indexed.

## Implementation
- Lightweight index in memory; fallback to server search for large catalogs
- Relevance boosts for exact SKU/spec matches

## Test Plan
- Perf: keystroke latency; relevance snapshot tests

## DoD
- [ ] Top‑N suggestions; spec fields searchable
- [ ] URL sync; back/forward navigation works
