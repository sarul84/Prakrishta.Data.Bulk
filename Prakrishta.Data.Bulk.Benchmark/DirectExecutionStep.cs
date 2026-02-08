using Prakrishta.Data.Bulk.Core;
using Prakrishta.Data.Bulk.Pipeline;
using System.Threading;
using System.Threading.Tasks;

public sealed class DirectExecutionStep : IBulkPipelineStep
{
    private readonly IBulkStrategy _strategy;
    private readonly string _connectionString;

    public DirectExecutionStep(IBulkStrategy strategy, string connectionString)
    {
        _strategy = strategy;
        _connectionString = connectionString;
    }

    public string Name => "StrategySelector";

    public Task ExecuteAsync(BulkContext context, CancellationToken cancellationToken)
    {
        return _strategy.ExecuteAsync(context, _connectionString, cancellationToken);
    }
}