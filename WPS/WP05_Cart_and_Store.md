# WP05 — Cart & Store

**Epic:** Checkout & Payments | **Priority:** 5 | **Complexity:** Easy

## Intent
Fast, reliable client cart with session persistence and currency awareness.

## Scope
- Add/remove/update qty; persist in localStorage/MemoryCache
- Currency-aware totals; empty state UX

## Stories & AC
- **Visitor:** “Cart survives refresh.” → **AC:** Reload retains items and qtys.
- **Owner:** “Cart events captured.” → **AC:** `cart.add`, `cart.remove`, `cart.update` emitted.

## Implementation
- Pure reducer/store with selectors, serialization; time‑boxed rehydration
- Guard against stale pricing (refresh on resume/focus)

## Test Plan
- Unit: reducers/selectors; serialization
- E2E: add→refresh→checkout happy path

## DoD
- [ ] Persisted cart, currency‑aware
- [ ] Events emitted; empty/error states
