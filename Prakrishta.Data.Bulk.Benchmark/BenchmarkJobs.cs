using BenchmarkDotNet.Engines;

namespace Prakrishta.Data.Bulk.Benchmarks;

public static class BenchmarkJobs
{
    public const RunStrategy Strategy = RunStrategy.Throughput;

    public const int LaunchCount = 1;
    public const int WarmupCount = 1;
    public const int IterationCount = 5;
    public const int InvocationCount = 1;
}