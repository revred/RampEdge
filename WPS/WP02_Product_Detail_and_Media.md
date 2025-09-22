# WP02 — Product Detail & Media

**Epic:** Product & Content Management | **Priority:** 5 | **Complexity:** Easy–Medium

## Intent
Deliver rich product detail pages/cards: images, videos, downloadable PDFs, structured specs with units/tolerances.

## Scope
- `<ProductDetail/>` (page/fragment), `<ProductGallery/>`, spec tables, download links
- Media handling with lazy loading; PDF open in new tab; file-type icons

## Stories & AC
- **Visitor:** “I can zoom images and swipe gallery on mobile.”  
  **AC:** Pinch/zoom (desktop zoom on hover); swipe works; images have alt text.
- **Engineer:** “I can download the PDF datasheet.”  
  **AC:** Click → opens PDF in new tab; event `asset.download` fired.
- **Owner:** “Add a media URL and it displays safely.”  
  **AC:** Only whitelisted MIME/image origins load; broken images handled.

## Implementation Notes
- Fixed aspect ratio wrappers to avoid CLS; IntersectionObserver for lazy
- Support `media[]`: `{url, kind:image|video|pdf, alt}`
- Spec table supports `{key, value, unit}` rendering with localization

## Security & A11y
- Sanitize captions/HTML; never `innerHTML` for untrusted content
- Keyboard nav: left/right in gallery; Esc closes lightbox

## Test Plan
- Unit: media renderer, spec formatter
- Manual: device matrix (iOS/Android/desktop) for gallery
- E2E: PDF downloads tracked; 404 media fallback

## DoD
- [ ] Detail + gallery render multiple media types
- [ ] Lazy images, no CLS, a11y labels provided
- [ ] Events logged for view/download
