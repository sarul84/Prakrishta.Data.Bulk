using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;

public sealed class BenchmarkConfig : ManualConfig
{
    public BenchmarkConfig()
    {
        AddJob(Job.Default
            .WithWarmupCount(2)
            .WithIterationCount(5)
            .WithLaunchCount(1));

        AddDiagnoser(MemoryDiagnoser.Default);
    }
}