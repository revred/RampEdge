# WP01: Foundation SQLite MinAPI

## Overview
VTEX-style e-commerce backend with SQLite database and minimal API endpoints for products, pricing, orders, and marketplace functionality.

## Scope
- SQLite database with 6 core tables
- RESTful API endpoints for CRUD operations
- Dynamic pricing with volume and segment discounts
- Marketplace catalog with token-based access control
- Order management with timeline tracking
- CRM events logging
- TypeScript SDK for client integration

## Features Implemented

### Core Tables
- `products` - Product catalog with JSON data storage
- `orders` - Order management with timeline tracking
- `discount_policies` - Volume and segment-based pricing rules
- `marketplace_exports` - Reseller catalog configurations
- `crm_events` - Customer relationship tracking
- `api_tokens` - Scoped authentication tokens

### API Endpoints
- `GET /api/ok` - Health check
- `GET /api/products` - List products with search
- `GET /api/products/{id}` - Get single product
- `POST /api/products` - Create/update product
- `GET /api/pricing/quote` - Dynamic pricing with discounts
- `POST /api/orders` - Create order
- `GET /api/orders/{id}` - Get order details
- `POST /api/checkout/session` - Create payment session (stub)

### Technology Stack
- **Backend**: ASP.NET Core 9.0 Minimal APIs
- **Database**: SQLite with JSON columns
- **Serialization**: System.Text.Json (fully compliant)
- **Authentication**: Bearer token with scoped permissions
- **SDK**: TypeScript with typed interfaces

## Usage

### Quickstart
1) Run the API: `dotnet run --project api`
2) Seed demo data: `powershell -f ops/seed.ps1`
3) Hit health: `curl http://localhost:5000/api/ok`
4) Browse products: `curl http://localhost:5000/api/products`
5) Get a quote: `curl "http://localhost:5000/api/pricing/quote?sku=AXLE-001&qty=12&segment=reseller"`
6) Marketplace (token):
   `curl -H "Authorization: Bearer tok_hippohex_demo" "http://localhost:5000/api/marketplace/catalog?reseller=HIPPOHEX-UK"`

### Demo Data
- **Products**: Precision axle, ball bearing, machine setup service
- **Tokens**: `tok_public_read`, `tok_hippohex_demo`
- **Discounts**: 5% at 10+ qty, 10% at 25+ qty, 12% for resellers, 7% for VIP
- **Marketplace**: HIPPOHEX-UK reseller with 2 products

## Architecture Benefits
- **Lightweight**: Single file API with embedded SQLite
- **Scalable**: JSON columns allow flexible product data
- **Secure**: Token-based authentication with granular scopes
- **Fast**: Minimal overhead with direct SQLite queries
- **Modern**: Uses latest .NET 9.0 and System.Text.Json

## Integration
The TypeScript SDK provides a complete client interface:

```typescript
import { listProducts, quote, createOrder } from './sdk';

// Browse products
const products = await listProducts('axle');

// Get pricing
const pricing = await quote('AXLE-001', 10, 'reseller');

// Create order
const order = await createOrder({
  id: 'ORD-001',
  customerRef: 'customer@company.com',
  items: [{ sku: 'AXLE-001', qty: 10, price: pricing.finalPrice }],
  total: pricing.finalPrice * 10
});
```

## Risks / Notes
- Keep single DB connection per request; no migrations engine yet.
- Keep everything synchronous or simple async for clarity.
- SQLite suitable for MVP; consider PostgreSQL for production scale.