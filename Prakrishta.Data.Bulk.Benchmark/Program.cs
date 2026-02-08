using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;

internal class Program
{
    public static void Main(string[] args)
    {
        var config = ManualConfig
            .CreateEmpty()
            .WithOptions(ConfigOptions.DisableOptimizationsValidator)
            .WithOptions(ConfigOptions.JoinSummary);

        BenchmarkSwitcher
            .FromAssembly(typeof(Program).Assembly)
            .Run(args, config);
    }
}