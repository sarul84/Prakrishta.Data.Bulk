using Prakrishta.Data.Bulk.Core;

namespace Prakrishta.Data.Bulk.Diagnostics
{
    public sealed class ConsoleBulkDiagnosticsSink : IBulkDiagnosticsSink
    {
        public void OnStrategySelected(BulkContext context)
            => Console.WriteLine($"[Bulk] Strategy selected: {context.StrategyKind} for {context.OperationKind} on {context.TableName} ({context.ItemCount} items)");

        public void OnStepStarted(BulkContext context, string stepName)
            => Console.WriteLine($"[Bulk] Step started: {stepName}");

        public void OnStepCompleted(BulkContext context, string stepName, TimeSpan duration)
            => Console.WriteLine($"[Bulk] Step completed: {stepName} in {duration.TotalMilliseconds:N1} ms");

        public void OnError(BulkContext context, Exception exception)
            => Console.WriteLine($"[Bulk] Error in {context.OperationKind} on {context.TableName}: {exception.Message}");
    }

}
