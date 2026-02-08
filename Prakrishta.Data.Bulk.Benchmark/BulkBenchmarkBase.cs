using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prakrishta.Data.Bulk.Benchmarks.ComparisonBenchmarks;

[SimpleJob(
    BenchmarkJobs.Strategy,
    launchCount: BenchmarkJobs.LaunchCount,
    warmupCount: BenchmarkJobs.WarmupCount,
    iterationCount: BenchmarkJobs.IterationCount,
    invocationCount: BenchmarkJobs.InvocationCount)]
public abstract class BulkBenchmarkBase
{
    protected BenchmarkHarness Harness = null!;
    protected List<TestEntity> Data = null!;
    protected const string Table = "dbo.TestEntities";

    [Params(1_000, 10_000, 50_000)]
    public int Count;

    [GlobalSetup]
    public async Task GlobalSetup()
    {
        Harness = new BenchmarkHarness();

        await Harness.CreateTableAsync(Table);
        await Harness.SeedAsync(Table, Count);

        Data = BenchmarkDataFactory.Create(Count);

        await OnGlobalSetupAsync();
    }

    protected virtual Task OnGlobalSetupAsync() => Task.CompletedTask;

    [IterationSetup]
    public void IterationSetup()
    {
        // For insert benchmarks, always start from empty table
        Harness.ExecuteNonQueryAsync($"TRUNCATE TABLE {Table};")
               .GetAwaiter().GetResult();
    }

    [GlobalCleanup]
    public async Task GlobalCleanup()
    {
        await OnGlobalCleanupAsync();
        await Harness.DisposeAsync();
    }

    protected virtual Task OnGlobalCleanupAsync() => Task.CompletedTask;
}