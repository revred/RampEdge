# WP03 — Pricing Rules & Discounts

**Epic:** Pricing & Discounts | **Priority:** 5 | **Complexity:** Medium

## Intent
Real‑time computation of final price from base → FX → rules → tax. Show transparent discounts on product cards and in cart.

## Scope
- Rules engine (volume breaks, partner tiers, coupons as future)
- Strike‑through display; per‑SKU/per‑category scopes

## Stories & AC
- **Owner:** “Configure 10+ price breaks.” → **AC:** Rules evaluate deterministically in <100ms.
- **Buyer:** “Quantity change updates price immediately.” → **AC:** Price recalculated on qty input; totals consistent.
- **Sales:** “Partner tier overrides apply.” → **AC:** Authenticated tier context changes effective price.

## Implementation
- Rule model: `{type: volume|tier|coupon, conditions, effect: percent|flat, scope}`
- Deterministic evaluation order; memoize by `(sku, qty, tier)`
- UI binds to selectors; emits `pricing.rule.applied`

## Test Plan
- Unit: rule evaluation matrix/fuzz
- E2E: cart→checkout totals parity
- Perf: p95 eval time budgeted <100ms

## DoD
- [ ] Engine + UI hooks integrated
- [ ] Strike‑through and final price consistent across list/card/cart
- [ ] Tests for overlap/edge cases; docs with JSON examples
