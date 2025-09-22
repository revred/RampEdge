# WP04 — Multi‑Currency & Tax Display

**Epic:** Pricing & Discounts | **Priority:** 4 | **Complexity:** Medium

## Intent
Display prices in visitor’s currency with correct symbols, rounding, and tax labels (incl/excl by locale).

## Scope
- Currency rates polling (<30m freshness), locale auto‑detect & toggle
- Tax badges: “incl. VAT” (GB/EU) vs “+ sales tax” (US) per policy

## Stories & AC
- **Visitor:** Toggle GBP/EUR/USD → **AC:** All visible prices switch, including cart and checkout.
- **Owner:** Configure tax display policy per country → **AC:** Labels consistent site‑wide.

## Implementation
- ISO currency formatting; round rules per currency
- Persist preference; SSR pre‑render currency when possible

## Test Plan
- Unit: FX math, rounding; symbol/format snapshots
- E2E: cart+checkout amounts match product displays

## DoD
- [ ] Rates freshness <30m; toggle persists
- [ ] Labels accurate; parity with checkout
