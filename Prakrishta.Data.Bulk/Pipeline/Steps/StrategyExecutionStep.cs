using Prakrishta.Data.Bulk.Core;
using Prakrishta.Data.Bulk.Enum;

namespace Prakrishta.Data.Bulk.Pipeline.Steps
{
    public sealed class StrategyExecutionStep : IBulkPipelineStep
    {
        private readonly IReadOnlyDictionary<BulkStrategyKind, IBulkStrategy> _strategies;
        private readonly string _connectionString;

        public string Name => "StrategyExecution";

        public StrategyExecutionStep(
            IEnumerable<IBulkStrategy> strategies,
            string connectionString)
        {
            _strategies = strategies.ToDictionary(s => s.Kind);
            _connectionString = connectionString;
        }

        public async Task ExecuteAsync(BulkContext context, CancellationToken cancellationToken)
        {
            if (!_strategies.TryGetValue(context.StrategyKind, out var strategy))
                throw new InvalidOperationException($"No strategy registered for {context.StrategyKind}.");

            var affected = await strategy.ExecuteAsync(context, _connectionString, cancellationToken);
            context.Properties["AffectedRows"] = affected;
        }
    }
}
