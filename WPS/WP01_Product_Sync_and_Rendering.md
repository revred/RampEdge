# WP01 — Product Sync and Rendering (Full WPS)

**Epic:** Product & Content Management  
**Work Package:** `WP01_Product_Sync_and_Rendering`  
**Priority:** 5 (Core MVP) | **Complexity:** Medium | **Owner:** RampEdge Core Team

## 0) Intent & Rationale
Provide the **single source of product truth** from RampEdge/Maker APIs to the UI. This WP enables fast, reliable, accessible product lists/cards that keep the catalog current without manual edits. It is prerequisite for pricing, cart, checkout, analytics.

## 1) Scope
- API client (ETag/SWR), delta refresh (2s polling → SSE later)
- Components: `<ProductList/>`, `<ProductCard/>` (Blazor RCL planned; patterns apply to any frontend)
- Skeleton loaders, error/empty states, SSR hydration safety, A11y (AA)

## 2) User Stories & AC
- **Owner:** “When I update a product, visitors see it ≤2s.”  
  **AC:** Given CMS update → When page open → Then card/list content updates without reload in ≤2s.
- **Developer:** “Drop in a component and it renders.”  
  **AC:** Given `<ProductList PageSize=24/>` → Then grid renders responsive with paging and skeletons.
- **Visitor:** “See accurate spec/availability quickly.”  
  **AC:** LCP ≤2.5s on reference device; badges show availability/MOQ.

## 3) API (example)
`GET /v1/products?category=&page=&pageSize=`  (ETag support)  
`GET /v1/products/changes?since=etag|iso`

## 4) Implementation
- Read-through cache (IndexedDB/memory) + SWR revalidate on focus/interval
- View models map API → UI (title, summary, specs[{k,v,u}], media[], availability)
- Grid: 2–3–4 cols by breakpoint; reserve image height to avoid CLS
- Events: `product.view`, `product.card.click`, `product.delta.applied`

## 5) Performance & A11y
- Budgets: LCP ≤2.5s; per-surface JS ≤20KB gz; P95 list TTFB (cached) <250ms
- A11y: roles list/listitem, keyboard traversal, alt text, focus visible

## 6) Test Plan
- Unit: model mapping; cache & ETag logic; store reducers/selectors
- E2E: list → delta update in ≤2s; error boundary recovers; skeletons → content
- A11y: axe pass ≥90; manual keyboard traversal
- Perf: Lighthouse budget gates on CI

## 7) Definition of Done
- [ ] Live API integration; delta refresh ≤2s
- [ ] Responsive grid/cards without CLS; skeletons implemented
- [ ] Events emitted; docs + examples included
- [ ] Tests pass (unit+E2E), budgets met, a11y AA

## 8) Example (Blazor)
```razor
@page "/catalog"
<RampEdge.ProductList PageSize="24" />
<RampEdge.ProductCard Sku="VALVE-16MM" />
```
