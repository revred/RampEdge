Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$root = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path)
$db = Join-Path $root "rampedge.db"

Write-Host "Seeding SQLite at $db"

# Ensure SQLite exists - if not, provide helpful error
try {
    sqlite3 --version | Out-Null
} catch {
    Write-Error "sqlite3 command not found. Please install SQLite3 CLI tools."
    exit 1
}

sqlite3 $db "PRAGMA journal_mode=WAL;"

# Products
Write-Host "Importing products..."
$prod = Get-Content (Join-Path $root "schema\seed.products.json") -Raw | ConvertFrom-Json
foreach ($p in $prod) {
  $data = ($p | ConvertTo-Json -Depth 8 -Compress).Replace("'","''")
  $sql = "INSERT OR REPLACE INTO products(id,sku,title,kind,data) VALUES('$($p.id)','$($p.sku)','$($p.title)','$($p.kind)','$data')"
  sqlite3 $db $sql
}

# Discounts
Write-Host "Importing discount policies..."
$d = Get-Content (Join-Path $root "schema\seed.discounts.json") -Raw | ConvertFrom-Json
foreach ($p in $d.policies) {
  $rules = ($p.rules | ConvertTo-Json -Depth 8 -Compress).Replace("'","''")
  $sql = "INSERT OR REPLACE INTO discount_policies(id,name,active,rules) VALUES('$($p.id)','$($p.name)',$($p.active),'$rules')"
  sqlite3 $db $sql
}

# Marketplace
Write-Host "Importing marketplace exports..."
$m = Get-Content (Join-Path $root "schema\seed.marketplace.json") -Raw | ConvertFrom-Json
foreach ($e in $m.exports) {
  $skus = ($e.skus | ConvertTo-Json -Depth 8 -Compress).Replace("'","''")
  $sql = "INSERT OR REPLACE INTO marketplace_exports(id,reseller_code,skus,token) VALUES('$($e.id)','$($e.reseller_code)','$skus','$($e.token)')"
  sqlite3 $db $sql
}

# Tokens
Write-Host "Importing tokens..."
$t = Get-Content (Join-Path $root "schema\seed.tokens.json") -Raw | ConvertFrom-Json
foreach ($row in $t.tokens) {
  $sql = "INSERT OR REPLACE INTO api_tokens(token,label,scopes) VALUES('$($row.token)','$($row.label)','$($row.scopes)')"
  sqlite3 $db $sql
}

Write-Host "Done."
Write-Host ""
Write-Host "Demo tokens:"
Write-Host "  - tok_public_read"
Write-Host "  - tok_hippohex_demo"
Write-Host ""
Write-Host "Try:"
Write-Host "  curl http://localhost:5000/api/ok"
Write-Host "  curl http://localhost:5000/api/products"
Write-Host "  curl 'http://localhost:5000/api/pricing/quote?sku=AXLE-001&qty=12&segment=reseller'"
Write-Host "  curl -H 'Authorization: Bearer tok_hippohex_demo' 'http://localhost:5000/api/marketplace/catalog?reseller=HIPPOHEX-UK'"