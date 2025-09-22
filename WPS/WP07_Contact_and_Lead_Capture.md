# WP07 — Contact & Lead Capture

**Epic:** CRM & Lead Capture | **Priority:** 4 | **Complexity:** Easy

## Intent
Drop‑in contact/quote form that posts to CRM with GDPR consent and notifications.

## Scope
- `<ContactForm/>` component; server endpoint; consent text versioning
- Notifications: email/Slack within 1 min

## Stories & AC
- **Visitor:** Submit form → **AC:** Lead appears in CRM; thank‑you UX shown.
- **Owner:** Consent captured → **AC:** Consent text + ts stored.

## Implementation
- Honeypot + optional reCAPTCHA; throttling (429) with friendly UX
- Event `lead.submit` with minimal PII

## Test Plan
- Unit: schema validation; throttle behavior
- E2E: submission happy path; consent captured

## DoD
- [ ] CRM entry + notifications wired
- [ ] GDPR consent persisted; anti‑spam in place
