using BenchmarkDotNet.Attributes;
using Prakrishta.Data.Bulk;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[Config(typeof(BenchmarkConfig))]
public class StoredProc_Benchmarks
{
    private BenchmarkHarness _harness = null!;
    private BulkEngine _engine = null!;
    private List<TestEntity> _data = null!;

    private const string Table = "dbo.TestEntities";
    private const string Type = "dbo.TestEntityType";
    private const string ProcInsert = "dbo.InsertTestEntities_TVP";
    private const string ProcUpdate = "dbo.UpdateTestEntities_TVP";
    private const string ProcDelete = "dbo.DeleteTestEntities_TVP";

    [Params(1_000, 10_000, 50_000)]
    public int Count;

    [GlobalSetup]
    public void GlobalSetup()
    {
        _harness = new BenchmarkHarness();
        _harness.CreateTableAsync("dbo.TestEntities").GetAwaiter().GetResult();
        _harness.EnsureTvpInfrastructureAsync().GetAwaiter().GetResult();

        _engine = BulkEngineFactory.Create(_harness.ConnectionString, stagingThreshold: 1);
        _data = BenchmarkDataFactory.Create(Count);
    }

    [IterationSetup]
    public void IterationSetup()
    {
        _harness.ExecuteNonQueryAsync("TRUNCATE TABLE dbo.TestEntities;")
                .GetAwaiter().GetResult();
    }


    [Benchmark]
    public Task Insert_TVP() =>
        _engine.InsertAsync(_data, Table, Type, ProcInsert);

    [Benchmark]
    public Task Update_TVP() =>
        _engine.UpdateAsync(_data, Table, Type, ProcUpdate);

    [Benchmark]
    public Task Delete_TVP() =>
        _engine.DeleteAsync(_data, Table, Type, ProcDelete);

    [GlobalCleanup]
    public async Task Cleanup() =>
        await _harness.DisposeAsync();
}