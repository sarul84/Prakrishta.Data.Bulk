namespace Prakrishta.Data.Bulk.Pipeline.Steps
{
    using Prakrishta.Data.Bulk.Core;
    using Prakrishta.Data.Bulk.Diagnostics;
    using Prakrishta.Data.Bulk.Pipeline.StrategySelector;

    public sealed class StrategySelectionStep(
        BulkStrategySelector selector
        , IBulkDiagnosticsSink diagnostics) : IBulkPipelineStep
    {
        private readonly BulkStrategySelector _selector = selector;
        private readonly IBulkDiagnosticsSink _diagnostics = diagnostics;

        public string Name => "StrategySelection";

        public Task ExecuteAsync(BulkContext context, CancellationToken cancellationToken)
        {
            context.StrategyKind = _selector.Select(context);
            _diagnostics.OnStrategySelected(context);
            return Task.CompletedTask;
        }
    }
}
