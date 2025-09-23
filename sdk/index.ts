export type QuoteResult = { sku:string; listPrice:number; discountPct:number; finalPrice:number; currency:string };
export type Product = { id:string; sku:string; title:string; kind:'part'|'service'; content:any; specs:any; pricing:{list:number; currency:string} };
export type Order = { id:string; customerRef:string; items:{sku:string; qty:number; price:number}[]; total:number; status:string; timeline:any[] };

async function req(path:string, init?:RequestInit) {
  const r = await fetch(path, { headers: { "Content-Type":"application/json", ...(init?.headers||{}) }, ...init });
  if (!r.ok) throw new Error(`${r.status} ${await r.text()}`);
  return r.json();
}

export const listProducts = (q?:string):Promise<Product[]> =>
  req(`/api/products${q?`?search=${encodeURIComponent(q)}`:''}`);

export const getProduct = (id:string):Promise<Product> =>
  req(`/api/products/${id}`);

export const upsertProduct = (p:Product):Promise<any> =>
  req(`/api/products`, { method:'POST', body: JSON.stringify(p) });

export const quote = (sku:string, qty:number, segment?:string):Promise<QuoteResult> =>
  req(`/api/pricing/quote?sku=${encodeURIComponent(sku)}&qty=${qty}&segment=${encodeURIComponent(segment||'')}`);

export const marketplaceCatalog = (reseller:string, token:string):Promise<Product[]> =>
  req(`/api/marketplace/catalog?reseller=${encodeURIComponent(reseller)}`, { headers: { Authorization:`Bearer ${token}` }});

export const createOrder = (order:Order):Promise<any> =>
  req(`/api/orders`, { method:'POST', body: JSON.stringify(order) });

export const getOrder = (id:string):Promise<Order> =>
  req(`/api/orders/${encodeURIComponent(id)}`);

export const appendOrderEvent = (id:string, eventName:string, meta?:any):Promise<any> =>
  req(`/api/orders/${encodeURIComponent(id)}/event`, { method:'POST', body: JSON.stringify({ event: eventName, meta }) });

export const createCheckoutSession = (orderId:string):Promise<{paymentUrl:string}> =>
  req(`/api/checkout/session`, { method:'POST', body: JSON.stringify({ orderId }) });

export const logCrmEvent = (id:string, account:string, kind:string, data:any):Promise<any> =>
  req(`/api/crm/events`, { method:'POST', body: JSON.stringify({ id, account, kind, data }) });

export const listCrmEvents = (account?:string):Promise<any[]> =>
  req(`/api/crm/events${account?`?account=${encodeURIComponent(account)}`:''}`);