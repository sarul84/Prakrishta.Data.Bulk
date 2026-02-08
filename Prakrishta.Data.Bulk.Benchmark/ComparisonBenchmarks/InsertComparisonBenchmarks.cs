using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using Prakrishta.Data.Bulk;
using Prakrishta.Data.Bulk.Benchmarks.ComparisonBenchmarks;
using System.Threading.Tasks;
using EFCore.BulkExtensions;

public class InsertComparisonBenchmarks : BulkBenchmarkBase
{
    private BulkEngine _engine = null!;
    private DbContext _efContext = null!;

    [GlobalSetup(Target = nameof(Prakrishta_Insert))]
    public void SetupPrakrishta()
    {
        _engine = BulkEngineFactory.CreateTruncateOnly(Harness.ConnectionString);
    }

    [GlobalSetup(Target = nameof(EFCore_Insert))]
    public void SetupEFCore()
    {
        _efContext = new TestDbContext(Harness.ConnectionString);
    }

    [Benchmark]
    public Task Prakrishta_Insert() =>
        _engine.InsertAsync(Data, Table);

    [Benchmark]
    public Task EFCore_Insert() =>
        _efContext.BulkInsertAsync(Data);
}