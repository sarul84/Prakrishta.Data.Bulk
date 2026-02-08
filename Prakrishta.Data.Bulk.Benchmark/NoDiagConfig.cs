using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;

public class NoDiagnosticsConfig : ManualConfig
{
    public NoDiagnosticsConfig()
    {
        var job = Job.Default
            .WithStrategy(BenchmarkDotNet.Engines.RunStrategy.Throughput)
            .WithLaunchCount(1)
            .WithWarmupCount(1)
            .WithIterationCount(5)
            .WithInvocationCount(1)
            .WithUnrollFactor(1);

        AddJob(job);
    }
}