# WP08 — CRM Timeline & Events

**Epic:** CRM & Lead Capture | **Priority:** 4 | **Complexity:** Medium

## Intent
Record visitor actions (downloads, views, form submits) into CRM timeline for sales follow‑up.

## Scope
- Event emitters + server sink; retries/idempotency; privacy filters

## Stories & AC
- **Sales:** See datasheet download history → **AC:** Timeline shows event with timestamps.

## Implementation
- Event schema versioned; PII minimized; batch with backoff
- Download click → `asset.download`; form submit → `lead.submit`

## Test Plan
- Integration: event → sink → CRM
- Fault injection: network failures retry

## DoD
- [ ] Timeline populated accurately
- [ ] Privacy checks + idempotent writes
