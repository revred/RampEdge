# WP11 — Presentation & Responsive UI

**Epic:** Presentation & UX | **Priority:** 3 | **Complexity:** Easy

## Intent
Polished, mobile‑first cards/gallery with availability/MOQ badges and consistent spacing/typography.

## Scope
- Tailwind utility classes; skeletons; focus rings; prefers‑reduced‑motion

## Stories & AC
- **Visitor:** “It looks professional and fast.” → **AC:** Lighthouse A11y ≥90; no layout shift.
- **Owner:** “Just drop it in.” → **AC:** Works without custom CSS.

## Implementation
- Content‑visibility; image placeholders with fixed ratio
- Badge palette with AA contrast; legend in docs

## Test Plan
- Visual regression; Lighthouse budget

## DoD
- [ ] A11y ≥90; CLS <0.1; skeletons implemented
- [ ] Badges accurate; styles documented
