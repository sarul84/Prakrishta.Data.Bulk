using BenchmarkDotNet.Attributes;
using Prakrishta.Data.Bulk.Benchmarks.ComparisonBenchmarks;
using System.Threading.Tasks;

namespace Prakrishta.Data.Bulk.Benchmarks.ComparisonBenchmarks;

public class Prakrishta_Insert_Benchmarks : BulkBenchmarkBase
{
    private BulkEngine _engine = null!;

    protected override Task OnGlobalSetupAsync()
    {
        _engine = BulkEngineFactory.CreateTruncateOnly(Harness.ConnectionString);
        return Task.CompletedTask;
    }

    [Benchmark(Baseline = true)]
    public Task Prakrishta_TruncateAndReload() =>
        _engine.InsertAsync(Data, Table);
}