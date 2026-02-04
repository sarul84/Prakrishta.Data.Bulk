using Prakrishta.Data.Bulk.Core;

namespace Prakrishta.Data.Bulk.Diagnostics
{
    public interface IBulkDiagnosticsSink
    {
        void OnStrategySelected(BulkContext context);
        void OnStepStarted(BulkContext context, string stepName);
        void OnStepCompleted(BulkContext context, string stepName, TimeSpan duration);
        void OnError(BulkContext context, Exception exception);
    }

}
