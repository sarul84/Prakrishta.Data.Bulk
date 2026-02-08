using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using Prakrishta.Data.Bulk;
using System.Collections.Generic;
using System.Threading.Tasks;

[SimpleJob(
    RunStrategy.Throughput,
    launchCount: 1,
    warmupCount: 1,
    iterationCount: 5,
    invocationCount: 1)]
[MinIterationCount(3)]
[MaxIterationCount(7)]
public class TruncateReload_Benchmarks
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
        await _harness.SeedAsync(Table, Count);

        _engine = BulkEngineFactory.CreateTruncateOnly(_harness.ConnectionString);

        _data = BenchmarkDataFactory.Create(Count);
    }

    [IterationSetup]
    public void IterationSetup()
    {
        _harness.ExecuteNonQueryAsync("TRUNCATE TABLE dbo.TestEntities;")
                .GetAwaiter().GetResult();
    }

    [Benchmark]
    public Task TruncateAndReload() =>
        _engine.InsertAsync(_data, Table);

    [GlobalCleanup]
    public async Task Cleanup() =>
        await _harness.DisposeAsync();
}