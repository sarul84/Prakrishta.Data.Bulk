# Prakrishta.Data.Bulk
High performance, extensible bulk operations for .NET. Prakrishta.Data.Bulk is a provider agnostic, pipeline based bulk engine designed for speed, flexibility, and testability. It complements Prakrishta.Data by enabling large scale insert, update, and delete operations with minimal overhead.

## Features

- Fastest‑in‑class bulk insert performance
- TVP‑based stored procedure strategy (fastest for pure inserts)
- Staging table strategy with MERGE (best for upsert/update/delete)
- Zero reflection, zero EF Core overhead
- Linear scaling from 1k → 50k+ rows
- Async/await support
- BenchmarkDotNet‑verified performance
- Works with SQL Server, Azure SQL, LocalDB
- Pure ADO.NET — no EF Core required
- Clean, extensible architecture

### Feature Comparison Table

|Feature  | Prakrishta (Stored Proc) | Prakrishta (Staging)  | EFCore.BulkExtensions  | Raw SqlBulkCopy |
|---------- |--------------|--------------|----------------|--------------- | 
| Bulk Insert |⭐ Fastest  |⭐ Very Fast  |Fast  |Fast  |
| Bulk Update | ❌ | ⭐ Yes (MERGE) | Yes | ❌ | 
| Bulk Delete | ❌ | ⭐ Yes (MERGE)  | Yes | ❌ | 
| Upsert  | ❌ | ⭐ Yes (MERGE) | Yes | ❌ |
| TVP Support | ⭐ Yes | Yes | Yes | No | 
| Reflection‑Free |⭐ Yes  | ⭐ Yes | ❌ No | Yes | 
| EF Core Required | No | No | Yes | No | 


## Getting Started (Quickstart Guide)

1. Install the NuGet package

```
dotnet add package Prakrishta.Data.Bulk
```

2. Define your entity

```
public sealed class SalesRecord
{
    public int Id { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal Amount { get; set; }
}
```

3. Create a bulk engine full example

```
var builder = WebApplication.CreateBuilder(args);

// Register Bulk Engine
builder.Services.AddBulkEngine(opts =>
{
    opts.DefaultStrategy = BulkStrategyKind.StoredProcedureTvp;
});

var app = builder.Build();

// Resolve engine
var bulk = app.Services.GetRequiredService<BulkEngine>();

// Sample data
var items = new List<SalesRecord>
{
    new() { Id = 1, SaleDate = DateTime.UtcNow, Amount = 100 },
    new() { Id = 2, SaleDate = DateTime.UtcNow, Amount = 200 }
};

// Insert
await bulk.InsertAsync(
    items,
    "dbo.Sales",
    "dbo.SalesType",
    "dbo.Sales_Insert");

// Partition Switch
await bulk.ReplacePartitionAsync(
    items,
    "dbo.FactSales",
    opts => opts
        .UseStagingTable("dbo.FactSales_Staging_7")
        .ForPartition(7));

app.Run();
```

4. Service Collection Extensions

```
using Microsoft.Extensions.DependencyInjection;
using Prakrishta.Data.Bulk.Abstractions;
using Prakrishta.Data.Bulk.Core;
using Prakrishta.Data.Bulk.Engine.Strategies;
using Prakrishta.Data.Bulk.Enum;

namespace Prakrishta.Data.Bulk.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBulkEngine(
        this IServiceCollection services,
        Action<BulkOptions>? configure = null)
    {
        // Options
        var options = new BulkOptions();
        configure?.Invoke(options);
        services.AddSingleton(options);

        // Factories
        services.AddSingleton<IBulkCopyFactory, SqlBulkCopyFactory>();
        services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();

        // Strategy selector
        services.AddSingleton<IBulkStrategySelector, BulkStrategySelector>();

        // Strategies
        services.AddSingleton<IBulkStrategy>(sp =>
            new StoredProcedureTvpStrategy(
                sp.GetRequiredService<IDbConnectionFactory>()));

        services.AddSingleton<IBulkStrategy>(sp =>
            new StagingTableStrategy(
                sp.GetRequiredService<IBulkCopyFactory>(),
                sp.GetRequiredService<IDbConnectionFactory>()));

        services.AddSingleton<IBulkStrategy>(sp =>
            new TruncateAndReloadStrategy(
                sp.GetRequiredService<IBulkCopyFactory>(),
                sp.GetRequiredService<IDbConnectionFactory>()));

        services.AddSingleton<IBulkStrategy>(sp =>
            new PartitionSwitchStrategy(
                sp.GetRequiredService<IBulkCopyFactory>(),
                sp.GetRequiredService<IDbConnectionFactory>()));

        // Strategy dictionary
        services.AddSingleton<IDictionary<BulkStrategyKind, IBulkStrategy>>(sp =>
        {
            var strategies = sp.GetServices<IBulkStrategy>();
            return strategies.ToDictionary(s => s.Kind, s => s);
        });

        // Pipeline
        services.AddSingleton<IBulkPipeline, BulkPipelineEngine>();

        // Engine
        services.AddSingleton<BulkEngine>();

        return services;
    }
}
```

5. TVP Type for SalesRecord

```
CREATE TYPE dbo.SalesType AS TABLE
(
    Id          INT            NOT NULL,
    SaleDate    DATETIME2(7)   NOT NULL,
    Amount      DECIMAL(18,2)  NOT NULL
);

```
✔ Must match your C# entity
✔ Must match your staging table
✔ Must NOT include identity or constraints

6. Auto‑Drop + Recreate Script

```
IF OBJECT_ID('dbo.SalesRecord_Staging', 'U') IS NOT NULL
    DROP TABLE dbo.SalesRecord_Staging;

CREATE TABLE dbo.SalesRecord_Staging
(
    Id          INT            NOT NULL,
    SaleDate    DATETIME2(7)   NOT NULL,
    Amount      DECIMAL(18,2)  NOT NULL
);

CREATE CLUSTERED INDEX IX_SalesRecord_Staging_Id
    ON dbo.SalesRecord_Staging (Id);
```


## Performance Benchmarks

| Rows |Prakrishta (Stored Proc) | Prakrishta (Staging) | Raw Sql | EFCore.BulkExtensions | Result |
|----------|-------------------|----------|-------------|----------|----------------------|
| 1,000  | 10.0 ms  | 12.8 ms  | 14.6 ms | 11.4 ms  | EFCore slightly faster |
| 10,000  |37.3 ms  | 48.2 ms  | 49.26 ms | 87.4 ms  | Prakrishta ~2× faster |
| 50,000  |188.0 ms | 195.0 ms  | 203.2 ms | 395.0 ms  | Prakrishta ~2× faster |

***Key Findings***

- EFCore.BulkExtensions performs well for small batches due to low setup overhead.
- Prakrishta’s stored-proc strategy is the fastest overall, especially at large batch sizes.
- Prakrishta’s staging-table strategy is also extremely fast and scales linearly.
- At 10k–50k rows, Prakrishta is 2× faster than EFCore.BulkExtensions.
- Staging-table strategy even outperforms raw SqlBulkCopy at large sizes.
- Performance is linear, predictable, and optimized for high-volume ingestion.

## Performance Chart (Markdown)
This chart visualizes the current benchmark results for 50,000 rows, the most meaningful scale for real‑world ETL and ingestion workloads.

Milliseconds (lower is better)

### Bulk Insert Performance (50,000 rows)

```
Prakrishta (Stored Proc)   | ████████████████████████████ 188 ms
Prakrishta (Staging)       | ██████████████████████████████ 195 ms
Raw SqlBulkCopy            | ███████████████████████████████ 203 ms
EFCore.BulkExtensions      | █████████████████████████████████████████████ 395 ms
```

### Bulk Insert Performance (10,000 rows)

```
Prakrishta (Stored Proc)   | ████████████████ 37.3 ms
Prakrishta (Staging)       | ████████████████████ 48.2 ms
Raw SqlBulkCopy            | ████████████████████ 49.2 ms
EFCore.BulkExtensions      | █████████████████████████████████ 87.4 ms
```

### Bulk Insert Performance (1,000 rows)

```
Prakrishta (Stored Proc)   | ████████ 10.0 ms
EFCore.BulkExtensions      | ████████ 11.4 ms
Prakrishta (Staging)       | █████████ 12.8 ms
Raw SqlBulkCopy            | ██████████ 14.6 ms
```

## Why Prakrishta Is Faster

Prakrishta.Data.Bulk achieves industry‑grade performance because it:
- Uses pure ADO.NET
- Avoids EF Core overhead
- Eliminates reflection
- Uses optimized TVP ingestion
- Uses linear‑scaling staging tables
- Minimizes SQL Server I/O and logging
- Reduces round trips
- Produces predictable, stable performance curves
  
This is why Prakrishta Data Bulk engine is:
- Faster than EFCore.BulkExtensions
- Faster than Raw SqlBulkCopy at scale
- The fastest overall at 50K rows (stored‑proc strategy)

## Choosing the Right Strategy
Different workloads benefit from different bulk‑loading strategies.
Prakrishta.Data.Bulk gives you three optimized paths — each designed for a specific class of problems.

### 1. Stored Procedure Strategy (TVP‑based) — Best Overall for Inserts

Use when you want:
- Maximum raw insert speed
- Minimal SQL Server overhead
- A single round‑trip to the database
- No MERGE logic
- No staging table
  
***Ideal for***:

- High‑volume inserts
- ETL ingestion
- Logging pipelines
- Append‑only tables
- Scenarios where the target table has no complex constraints
  
***Why choose it***:
  
Fastest strategy at 1k, 10k, and 50k rows.
Outperforms EFCore.BulkExtensions and even Raw SqlBulkCopy.

### 2. Staging Table Strategy — Best for Upserts, Updates & Deletes

Use when you need:
- MERGE semantics
- Update‑or‑insert behavior
- Delete‑or‑insert behavior
- Full control over matching keys
- Idempotent ingestion
  
***Ideal for***:

- Slowly changing dimensions (SCD)
- Data warehouse loads
- Sync jobs
- Reconciliation pipelines
- Any scenario requiring deterministic upsert logic
  
***Why choose it***:

Linear scaling, extremely stable, and 2× faster than EFCore.BulkExtensions at medium and large batch sizes.

### 3. Raw SqlBulkCopy Strategy — Baseline / Custom Scenarios

Use when you want:
- Absolute minimal overhead
- Full control over the SqlBulkCopy pipeline
- Custom batching or streaming logic
- No MERGE or stored proc logic
  
***Ideal for***:

- Internal pipelines
- Custom ETL frameworks
- Scenarios where you want to build your own logic on top of SqlBulkCopy

***Why choose it***:

Great baseline — and your strategies outperform it at scale.

### 4. When to Choose Which Strategy

|Scenario|Best Strategy|why|
|-----------|------------------|----------|
|Pure inserts|Stored Proc|Fastest end‑to‑end path|
|Inserts + updates|Staging|MERGE logic built‑in|
|Inserts + deletes|Staging|MERGE handles delete conditions|
|Large batch ingestion|Stored Proc / Staging|Both scale linearly|
|Small batch inserts|Stored Proc|Lowest overhead|
|EF Core replacement|Stored Proc / Staging|2× faster at scale|
|Custom pipelines|Raw SqlBulkCopy|Maximum control|


## Performance Badges

![Performance](https://img.shields.io/badge/Performance-2×%20Faster%20Than%20EFCore.BulkExtensions-brightgreen?style=for-the-badge)
![Speed](https://img.shields.io/badge/50k%20Insert-188ms-success?style=for-the-badge)
![Benchmark Winner](https://img.shields.io/badge/Benchmark%20Winner-Yes!-brightgreen?style=for-the-badge)


## License
MIT License — free for commercial and open‑source use.












