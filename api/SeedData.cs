using Microsoft.Data.Sqlite;
using System.Text.Json;

public class SeedData
{
    public static async Task SeedDatabase(string dbPath)
    {
        using var conn = new SqliteConnection($"Data Source={dbPath}");
        await conn.OpenAsync();

        // Create tables first
        var createCmd = conn.CreateCommand();
        createCmd.CommandText = @"
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
);";
        await createCmd.ExecuteNonQueryAsync();

        // Seed products
        var products = new object[]
        {
            new {
                id = "P-AXLE-001",
                sku = "AXLE-001",
                title = "Precision Axle 14mm",
                kind = "part",
                content = new { summary = "Hardened steel axle for OEM assemblies", media = Array.Empty<object>() },
                specs = new { od_mm = 14, length_mm = 250, material = "EN24T" },
                pricing = new { list = 29.9m, currency = "GBP" }
            },
            new {
                id = "P-BRG-6202",
                sku = "BRG-6202",
                title = "Ball Bearing 6202",
                kind = "part",
                content = new { summary = "Deep groove radial bearing", media = Array.Empty<object>() },
                specs = new { id_mm = 15, od_mm = 35, width_mm = 11 },
                pricing = new { list = 3.2m, currency = "GBP" }
            },
            new {
                id = "S-MACH-SETUP",
                sku = "SERV-SETUP",
                title = "Machine Setup Service",
                kind = "service",
                content = new { summary = "Initial setup and calibration for CNC cell", media = Array.Empty<object>() },
                specs = new { hours = 4, engineer_grade = "Senior" },
                pricing = new { list = 120.0m, currency = "GBP" }
            }
        };

        foreach (dynamic p in products)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT OR REPLACE INTO products(id,sku,title,kind,data) VALUES(@id,@sku,@title,@kind,@data)";
            cmd.Parameters.AddWithValue("@id", p.id);
            cmd.Parameters.AddWithValue("@sku", p.sku);
            cmd.Parameters.AddWithValue("@title", p.title);
            cmd.Parameters.AddWithValue("@kind", p.kind);
            cmd.Parameters.AddWithValue("@data", JsonSerializer.Serialize(p));
            await cmd.ExecuteNonQueryAsync();
        }

        // Seed discount policies
        var policy = new
        {
            id = "POLICY-DEFAULT",
            name = "Default Volume/Segment",
            active = 1,
            rules = new
            {
                volume = new[] { new { min = 10, pct = 5 }, new { min = 25, pct = 10 } },
                segments = new[] { new { code = "reseller", pct = 12 }, new { code = "vip", pct = 7 } }
            }
        };

        var policyCmd = conn.CreateCommand();
        policyCmd.CommandText = @"INSERT OR REPLACE INTO discount_policies(id,name,active,rules) VALUES(@id,@name,@active,@rules)";
        policyCmd.Parameters.AddWithValue("@id", policy.id);
        policyCmd.Parameters.AddWithValue("@name", policy.name);
        policyCmd.Parameters.AddWithValue("@active", policy.active);
        policyCmd.Parameters.AddWithValue("@rules", JsonSerializer.Serialize(policy.rules));
        await policyCmd.ExecuteNonQueryAsync();

        // Seed marketplace exports
        var export = new
        {
            id = "MKT-HIPPOHEX-UK",
            reseller_code = "HIPPOHEX-UK",
            skus = new[] { "AXLE-001", "BRG-6202" },
            token = "tok_hippohex_demo"
        };

        var exportCmd = conn.CreateCommand();
        exportCmd.CommandText = @"INSERT OR REPLACE INTO marketplace_exports(id,reseller_code,skus,token) VALUES(@id,@code,@skus,@token)";
        exportCmd.Parameters.AddWithValue("@id", export.id);
        exportCmd.Parameters.AddWithValue("@code", export.reseller_code);
        exportCmd.Parameters.AddWithValue("@skus", JsonSerializer.Serialize(export.skus));
        exportCmd.Parameters.AddWithValue("@token", export.token);
        await exportCmd.ExecuteNonQueryAsync();

        // Seed API tokens
        var tokens = new[]
        {
            new { token = "tok_public_read", label = "Public Read", scopes = "read" },
            new { token = "tok_hippohex_demo", label = "HippoHex Reseller", scopes = "marketplace:HIPPOHEX-UK,pricing:quote,read" }
        };

        foreach (var t in tokens)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = @"INSERT OR REPLACE INTO api_tokens(token,label,scopes) VALUES(@token,@label,@scopes)";
            cmd.Parameters.AddWithValue("@token", t.token);
            cmd.Parameters.AddWithValue("@label", t.label);
            cmd.Parameters.AddWithValue("@scopes", t.scopes);
            await cmd.ExecuteNonQueryAsync();
        }

        Console.WriteLine("Database seeded successfully!");
        Console.WriteLine("\nDemo tokens:");
        Console.WriteLine("  - tok_public_read");
        Console.WriteLine("  - tok_hippohex_demo");
        Console.WriteLine("\nTry:");
        Console.WriteLine("  curl http://localhost:5000/api/ok");
        Console.WriteLine("  curl http://localhost:5000/api/products");
        Console.WriteLine("  curl 'http://localhost:5000/api/pricing/quote?sku=AXLE-001&qty=12&segment=reseller'");
    }
}