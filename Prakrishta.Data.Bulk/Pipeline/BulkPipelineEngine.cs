using Prakrishta.Data.Bulk.Core;
using Prakrishta.Data.Bulk.Diagnostics;
using System.Diagnostics;

namespace Prakrishta.Data.Bulk.Pipeline
{
    public sealed class BulkPipelineEngine(
        IEnumerable<IBulkPipelineStep> steps,
        IBulkDiagnosticsSink? diagnostics = null): IBulkPipeline
    {
        private readonly IReadOnlyList<IBulkPipelineStep> _steps = steps.ToList();
        private readonly IBulkDiagnosticsSink _diagnostics = diagnostics ?? NullBulkDiagnosticsSink.Instance;

        public async Task<int> ExecuteAsync(BulkContext context, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var step in _steps)
                {
                    _diagnostics.OnStepStarted(context, step.Name);
                    var sw = Stopwatch.StartNew();

                    await step.ExecuteAsync(context, cancellationToken);

                    sw.Stop();
                    _diagnostics.OnStepCompleted(context, step.Name, sw.Elapsed);
                }

                return context.Properties.TryGetValue("AffectedRows", out var v) && v is int i ? i : 0;
            }
            catch (Exception ex)
            {
                _diagnostics.OnError(context, ex);
                throw;
            }
        }
    }

}
