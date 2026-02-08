# Prakrishta.Data.Bulk
High performance, extensible bulk operations for .NET. Prakrishta.Data.Bulk is a provider agnostic, pipeline based bulk engine designed for speed, flexibility, and testability. It complements Prakrishta.Data by enabling large scale insert, update, and delete operations with minimal overhead.

## Features

- Pipeline-based architecture
Add middleware for diagnostics, batching, logging, or custom behaviors.
- Strategy selector
Automatically chooses the best bulk strategy (SqlBulkCopy, staging table, etc.).
- Provider-agnostic abstractions
Fully fakeable ADO.NET layer for deterministic unit tests.
- High performance
Optimized POCO mapping, pooled buffers, and minimal allocations.
- Extensible
Add your own strategies, pipeline steps, or diagnostics collectors.

## Installation

```
dotnet add package Prakrishta.Data.Bulk
```

## Performance Summary
We compared the Prakrishta.Data.Bulk staging‑table insert strategy against EFCore.BulkExtensions using BenchmarkDotNet on .NET 8 with LocalDB.

| Rows | Prakrishta (Staging) | Raw Sql | EFCore.BulkExtensions | Result |
|----------|----------|-------------|----------|----------------------|
| 1,000  | 12.8 ms  | 14.6 ms | 11.4 ms  | EFCore slightly faster |
| 10,000  | 48.2 ms  | 49.26 ms | 87.4 ms  | Prakrishta ~2× faster |
| 50,000  | 195.0 ms  | 203.2 ms | 395.0 ms  | Prakrishta ~2× faster |



Key Findings

- EFCore.BulkExtensions is slightly faster for very small batches due to lower setup overhead.
- Prakrishta.Data.Bulk becomes significantly faster as batch size increases.
- At 10k–50k rows, Prakrishta is 2× faster than EFCore.BulkExtensions.
- Performance is more stable and predictable due to pure ADO.NET execution.
- Staging‑table strategy scales linearly and is ideal for high‑volume ETL workloads.




