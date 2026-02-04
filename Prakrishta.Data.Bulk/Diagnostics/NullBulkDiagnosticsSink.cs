using Prakrishta.Data.Bulk.Core;

namespace Prakrishta.Data.Bulk.Diagnostics
{
    public sealed class NullBulkDiagnosticsSink : IBulkDiagnosticsSink
    {
        public static readonly IBulkDiagnosticsSink Instance = new NullBulkDiagnosticsSink();
        private NullBulkDiagnosticsSink() { }

        public void OnStrategySelected(BulkContext context) { }
        public void OnStepStarted(BulkContext context, string stepName) { }
        public void OnStepCompleted(BulkContext context, string stepName, TimeSpan duration) { }
        public void OnError(BulkContext context, Exception exception) { }
    }

}
