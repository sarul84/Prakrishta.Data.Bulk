# Prakrishta.Data.Bulk
High?performance, extensible bulk operations for .NET. Prakrishta.Data.Bulk is a provider?agnostic, pipeline?based bulk engine designed for speed, flexibility, and testability. It complements Prakrishta.Data by enabling large?scale insert, update, and delete operations with minimal overhead.

##Features

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




