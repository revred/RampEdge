using Microsoft.Data.Sqlite;
using System.Text.Json;
using System.Text.Json.Serialization;

// Handle seeding if requested
if (args.Length > 0 && args[0] == "--seed")
{
    var seedDbPath = Path.Combine(AppContext.BaseDirectory, "..", "rampedge.db");
    Directory.CreateDirectory(Path.GetDirectoryName(seedDbPath)!);
    await SeedData.SeedDatabase(seedDbPath);
    return;
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
});

var dbPath = Path.Combine(AppContext.BaseDirectory, "..", "rampedge.db");
Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
builder.Services.AddSingleton(new SqliteConnectionStringBuilder { DataSource = dbPath }.ToString());

var app = builder.Build();

// Ensure DB & tables
using (var conn = new SqliteConnection(app.Services.GetRequiredService<string>()))
{
    conn.Open();
    var cmd = conn.CreateCommand();
    cmd.CommandText = @"
CREATE TABLE IF NOT EXISTS products(
  id TEXT PRIMARY KEY,
  sku TEXT UNIQUE,
  title TEXT,
  kind TEXT,
  data TEXT,
  created_at TEXT DEFAULT (datetime('now'))
);
CREATE TABLE IF NOT EXISTS discount_policies(
  id TEXT PRIMARY KEY,
  name TEXT,
  active INTEGER,
  rules TEXT
);
CREATE TABLE IF NOT EXISTS marketplace_exports(
  id TEXT PRIMARY KEY,
  reseller_code TEXT,
  skus TEXT,
  token TEXT
);
CREATE TABLE IF NOT EXISTS orders(
  id TEXT PRIMARY KEY,
  customer_ref TEXT,
  items TEXT,
  total REAL,
  status TEXT,
  timeline TEXT,
  created_at TEXT DEFAULT (datetime('now'))
);
CREATE TABLE IF NOT EXISTS crm_events(
  id TEXT PRIMARY KEY,
  account TEXT,
  kind TEXT,
  data TEXT,
  ts TEXT DEFAULT (datetime('now'))
);
CREATE TABLE IF NOT EXISTS api_tokens(
  token TEXT PRIMARY KEY,
  label TEXT,
  scopes TEXT
);
";
    cmd.ExecuteNonQuery();
}

app.MapGet("/api/ok", () => Results.Json(new { ok = true }));

// -------------------
// Helpers
// -------------------
static string Json(object o, JsonSerializerOptions jso) => JsonSerializer.Serialize(o, jso);
static T? FromJson<T>(string s) => JsonSerializer.Deserialize<T>(s);

static bool HasScope(HttpRequest req, string needed, string? tokenScopes)
{
    if (string.IsNullOrWhiteSpace(needed)) return true;
    if (string.IsNullOrWhiteSpace(tokenScopes)) return false;
    var scopes = tokenScopes.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
    if (needed.EndsWith(":*"))
    {
        var prefix = needed[..^2];
        return scopes.Any(s => s.StartsWith(prefix));
    }
    return scopes.Contains(needed);
}

async Task<(bool ok, string? scopes)> ValidateToken(HttpRequest req, SqliteConnection conn)
{
    var auth = req.Headers.Authorization.ToString();
    if (string.IsNullOrWhiteSpace(auth) || !auth.StartsWith("Bearer ")) return (false, null);
    var token = auth["Bearer ".Length..].Trim();
    var get = conn.CreateCommand();
    get.CommandText = "SELECT scopes FROM api_tokens WHERE token=@t";
    get.Parameters.AddWithValue("@t", token);
    var scopes = (string?)await get.ExecuteScalarAsync();
    return (scopes != null, scopes);
}

// -------------------
// Products
// -------------------
app.MapGet("/api/products", async (HttpRequest req, string connStr) =>
{
    var q = req.Query["search"].ToString();
    using var conn = new SqliteConnection(connStr);
    await conn.OpenAsync();
    var cmd = conn.CreateCommand();
    if (string.IsNullOrWhiteSpace(q))
    {
        cmd.CommandText = "SELECT id, sku, title, kind, data FROM products LIMIT 200";
    }
    else
    {
        cmd.CommandText = "SELECT id, sku, title, kind, data FROM products WHERE sku LIKE @q OR title LIKE @q LIMIT 200";
        cmd.Parameters.AddWithValue("@q", $"%{q}%");
    }
    using var r = await cmd.ExecuteReaderAsync();
    var list = new List<object>();
    while (await r.ReadAsync())
    {
        list.Add(new {
            id = r.GetString(0), sku = r.GetString(1), title = r.GetString(2), kind = r.GetString(3),
            content = FromJson<JsonElement>(r.GetString(4)).GetProperty("content"),
            specs = FromJson<JsonElement>(r.GetString(4)).GetProperty("specs"),
            pricing = FromJson<JsonElement>(r.GetString(4)).GetProperty("pricing")
        });
    }
    return Results.Json(list);
});

app.MapGet("/api/products/{id}", async (string id, string connStr) =>
{
    using var conn = new SqliteConnection(connStr);
    await conn.OpenAsync();
    var cmd = conn.CreateCommand();
    cmd.CommandText = "SELECT id, sku, title, kind, data FROM products WHERE id=@id";
    cmd.Parameters.AddWithValue("@id", id);
    using var r = await cmd.ExecuteReaderAsync();
    if (!await r.ReadAsync()) return Results.NotFound();
    var data = FromJson<JsonElement>(r.GetString(4));
    return Results.Json(new {
        id = r.GetString(0), sku = r.GetString(1), title = r.GetString(2), kind = r.GetString(3),
        content = data.GetProperty("content"),
        specs = data.GetProperty("specs"),
        pricing = data.GetProperty("pricing")
    });
});

app.MapPost("/api/products", async (HttpRequest req, string connStr, JsonSerializerOptions jso) =>
{
    using var doc = await JsonDocument.ParseAsync(req.Body);
    var root = doc.RootElement;
    string id = root.GetProperty("id").GetString()!;
    string sku = root.GetProperty("sku").GetString()!;
    string title = root.GetProperty("title").GetString()!;
    string kind = root.GetProperty("kind").GetString()!;
    // validate minimal presence
    if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(sku))
        return Results.BadRequest(new { error = "id and sku required" });

    using var conn = new SqliteConnection(connStr);
    await conn.OpenAsync();
    var up = conn.CreateCommand();
    up.CommandText = @"INSERT INTO products(id, sku, title, kind, data)
                       VALUES(@id,@sku,@title,@kind,@data)
                       ON CONFLICT(id) DO UPDATE SET sku=@sku,title=@title,kind=@kind,data=@data";
    up.Parameters.AddWithValue("@id", id);
    up.Parameters.AddWithValue("@sku", sku);
    up.Parameters.AddWithValue("@title", title);
    up.Parameters.AddWithValue("@kind", kind);
    up.Parameters.AddWithValue("@data", root.GetRawText());
    await up.ExecuteNonQueryAsync();
    return Results.Json(new { ok = true });
});

// -------------------
// Pricing / Quote
// -------------------

static decimal QuoteEval(decimal list, int qty, string? segment, JsonElement rules)
{
    decimal pct = 0m;

    // Volume discounts
    if (rules.TryGetProperty("volume", out var volumeEl))
    {
        foreach (var vol in volumeEl.EnumerateArray())
        {
            var min = vol.GetProperty("min").GetInt32();
            var volPct = vol.GetProperty("pct").GetDecimal();
            if (qty >= min) pct = Math.Max(pct, volPct);
        }
    }

    // Segment discounts
    if (!string.IsNullOrWhiteSpace(segment) && rules.TryGetProperty("segments", out var segmentEl))
    {
        foreach (var seg in segmentEl.EnumerateArray())
        {
            var code = seg.GetProperty("code").GetString();
            var segPct = seg.GetProperty("pct").GetDecimal();
            if (string.Equals(code, segment, StringComparison.OrdinalIgnoreCase))
                pct = Math.Max(pct, segPct);
        }
    }

    return Math.Round(list * (1 - pct / 100m), 2);
}

app.MapGet("/api/pricing/quote", async (HttpRequest req, string connStr) =>
{
    var sku = req.Query["sku"].ToString();
    var qty = int.TryParse(req.Query["qty"], out var qv) ? qv : 1;
    var segment = req.Query["segment"].ToString();
    using var conn = new SqliteConnection(connStr);
    await conn.OpenAsync();
    var p = conn.CreateCommand();
    p.CommandText = "SELECT data FROM products WHERE sku=@sku";
    p.Parameters.AddWithValue("@sku", sku);
    var pdata = (string?)await p.ExecuteScalarAsync();
    if (pdata is null) return Results.NotFound(new { error = "sku not found" });
    var pjson = FromJson<JsonElement>(pdata);
    var list = pjson.GetProperty("pricing").GetProperty("list").GetDecimal();
    var currency = pjson.GetProperty("pricing").GetProperty("currency").GetString()!;

    var d = conn.CreateCommand();
    d.CommandText = "SELECT rules FROM discount_policies WHERE active=1 LIMIT 1";
    var rulesStr = (string?)await d.ExecuteScalarAsync();
    var rules = rulesStr != null ? FromJson<JsonElement>(rulesStr) : new JsonElement();
    var final = QuoteEval(list, qty, segment, rules);

    return Results.Json(new { sku, listPrice = list, discountPct = Math.Round(100m * (list - final) / (list == 0 ? 1 : list), 2), finalPrice = final, currency });
});

// -------------------
// Orders
// -------------------
app.MapPost("/api/orders", async (HttpRequest req, string connStr) =>
{
    using var doc = await JsonDocument.ParseAsync(req.Body);
    var root = doc.RootElement;
    var id = root.GetProperty("id").GetString()!;
    var cust = root.GetProperty("customerRef").GetString()!;
    var items = root.GetProperty("items").GetRawText();
    var total = root.TryGetProperty("total", out var t) ? t.GetDecimal() : 0m;
    var timeline = JsonSerializer.Serialize(new[]{ new { ts=DateTime.UtcNow.ToString("o"), eventName="created", meta=(object?)null }});

    using var conn = new SqliteConnection(connStr);
    await conn.OpenAsync();
    var ins = conn.CreateCommand();
    ins.CommandText = @"INSERT INTO orders(id, customer_ref, items, total, status, timeline)
                        VALUES(@i,@c,@it,@tot,'new',@tl)";
    ins.Parameters.AddWithValue("@i", id);
    ins.Parameters.AddWithValue("@c", cust);
    ins.Parameters.AddWithValue("@it", items);
    ins.Parameters.AddWithValue("@tot", total);
    ins.Parameters.AddWithValue("@tl", timeline);
    await ins.ExecuteNonQueryAsync();
    return Results.Json(new { ok = true, id });
});

app.MapGet("/api/orders/{id}", async (string id, string connStr) =>
{
    using var conn = new SqliteConnection(connStr);
    await conn.OpenAsync();
    var cmd = conn.CreateCommand();
    cmd.CommandText = "SELECT id, customer_ref, items, total, status, timeline FROM orders WHERE id=@i";
    cmd.Parameters.AddWithValue("@i", id);
    using var r = await cmd.ExecuteReaderAsync();
    if (!await r.ReadAsync()) return Results.NotFound();
    return Results.Json(new {
        id=r.GetString(0), customerRef=r.GetString(1),
        items=FromJson<JsonElement>(r.GetString(2)),
        total=r.GetDouble(3), status=r.GetString(4),
        timeline=FromJson<JsonElement>(r.GetString(5))
    });
});

// -------------------
// Checkout (stub)
// -------------------
app.MapPost("/api/checkout/session", async (HttpRequest req) =>
{
    using var doc = await JsonDocument.ParseAsync(req.Body);
    var orderId = doc.RootElement.GetProperty("orderId").GetString()!;
    // In real life, generate Stripe/A2A link. For MVP return mock URL.
    return Results.Json(new { paymentUrl = $"https://example.test/pay/{orderId}" });
});

app.Run();