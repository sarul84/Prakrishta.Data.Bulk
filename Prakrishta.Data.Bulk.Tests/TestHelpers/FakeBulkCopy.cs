using Prakrishta.Data.Bulk.Abstractions;
using System.Data;

namespace Prakrishta.Data.Bulk.Tests.TestHelpers
{
    public sealed class FakeBulkCopy : IBulkCopy
    {
        public string DestinationTableName { get; set; } = "";
        public IList<(string Source, string Destination)> ColumnMappings { get; }
            = new List<(string, string)>();

        public DataTable? CapturedTable { get; private set; }
        public bool WasCalled { get; private set; }

        public Task WriteToServerAsync(DataTable table, CancellationToken cancellationToken)
        {
            WasCalled = true;
            CapturedTable = table;
            return Task.CompletedTask;
        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
