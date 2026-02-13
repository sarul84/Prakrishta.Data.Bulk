namespace Prakrishta.Data.Bulk.Pipeline.Steps
{
    using Prakrishta.Data.Bulk.Core;
    using Prakrishta.Data.Bulk.Enum;

    public sealed class StrategyExecutionStep(
        IEnumerable<IBulkStrategy> strategies,
        string connectionString) : IBulkPipelineStep
    {
        private readonly IReadOnlyDictionary<BulkStrategyKind, IBulkStrategy> _strategies = strategies.ToDictionary(s => s.Kind);
        private readonly string _connectionString = connectionString;

        public string Name => "StrategyExecution";

        public async Task ExecuteAsync(BulkContext context, CancellationToken cancellationToken)
        {
            if (!_strategies.TryGetValue(context.StrategyKind, out var strategy))
                throw new InvalidOperationException($"No strategy registered for {context.StrategyKind}.");

            var affected = await strategy.ExecuteAsync(context, _connectionString, cancellationToken);
            context.Properties["AffectedRows"] = affected;
        }
    }
}
