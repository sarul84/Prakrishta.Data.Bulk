using BenchmarkDotNet.Attributes;
using Prakrishta.Data.Bulk;
using System.Collections.Generic;
using System.Threading.Tasks;

[Config(typeof(BenchmarkConfig))]
public class StagingTableStrategy_UpdateDelete_Benchmarks
{
    private BenchmarkHarness _harness = null!;
    private BulkEngine _engine = null!;
    private List<TestEntity> _data = null!;
    private const string Table = "dbo.TestEntities";

    [Params(1_000, 10_000, 50_000)]
    public int Count;

    [GlobalSetup()]
    public async Task Setup()
    {
        _harness = new BenchmarkHarness();

        await _harness.CreateTableAsync(Table);
        await _harness.SeedAsync(Table, Count);

        _engine = BulkEngineFactory.Create(
            _harness.ConnectionString,
            stagingThreshold: 1
        );

        _data = BenchmarkDataFactory.Create(Count);
    }


    [Benchmark]
    public Task Update_Staging() =>
        _engine.UpdateAsync(_data, Table);

    [Benchmark]
    public Task Delete_Staging() =>
        _engine.DeleteAsync(_data, Table);

    [GlobalCleanup]
    public async Task Cleanup() =>
        await _harness.DisposeAsync();
}