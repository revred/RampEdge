# WP06 — Checkout (Stripe/SmartPay)

**Epic:** Checkout & Payments | **Priority:** 5 | **Complexity:** Med–Hard

## Intent
Complete payment in ≤3 clicks using provider adapters; hosted‑checkout fallback if embed blocked.

## Scope
- Provider adapters (Stripe Elements first), ephemeral payment intent, webhooks
- Success/Cancel redirects; receipt events

## Stories & AC
- **Buyer:** Pay with card/Apple Pay → **AC:** Charge succeeds and redirects to `SuccessUrl`.
- **Owner:** Hosted fallback if CSP blocks → **AC:** Same basket on hosted page.

## Implementation
- Server creates intents; client handles Elements & error states
- Webhook confirms order; emit `checkout.success|error`

## Security
- PCI delegated to provider; CSRF on server endpoints; token scopes minimal

## Test Plan
- Sandbox keys E2E; webhook replay tests; decline paths

## DoD
- [ ] Adapter runs with test keys end‑to‑end
- [ ] Fallback path documented and verified
