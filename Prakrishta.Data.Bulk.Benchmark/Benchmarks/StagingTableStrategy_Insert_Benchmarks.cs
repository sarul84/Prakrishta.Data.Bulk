using BenchmarkDotNet.Attributes;
using Prakrishta.Data.Bulk;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[Config(typeof(BenchmarkConfig))]
public class StagingTableStrategy_Insert_Benchmarks
{
    private BenchmarkHarness _harness = null!;
    private BulkEngine _engine = null!;
    private List<TestEntity> _data = null!;
    private const string Table = "dbo.TestEntities";

    [Params(1_000, 10_000, 50_000)]
    public int Count;

    [GlobalSetup]
    public async Task Setup()
    {
        _harness = new BenchmarkHarness();
        await _harness.CreateTableAsync(Table);

        var count = await _harness.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM {Table}");
        Console.WriteLine("Row count BEFORE INSERT = " + count);

        _engine = BulkEngineFactory.Create(_harness.ConnectionString, stagingThreshold: 1);
        _data = BenchmarkDataFactory.Create(Count);
    }

    [IterationSetup]
    public void IterationSetup()
    {
        _harness.ExecuteNonQueryAsync($"TRUNCATE TABLE {Table};")
            .GetAwaiter().GetResult();

        var count = _harness.ExecuteScalarAsync<int>($"SELECT COUNT(*) FROM {Table}")
                            .GetAwaiter().GetResult();
    }

    [Benchmark]
    public Task Insert_Staging() =>
        _engine.InsertAsync(_data, Table);
}
