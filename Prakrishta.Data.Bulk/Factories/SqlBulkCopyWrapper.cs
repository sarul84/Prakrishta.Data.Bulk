namespace Prakrishta.Data.Bulk.Factories
{
    using Prakrishta.Data.Bulk.Abstractions;
    using System.Data;

    public sealed class SqlBulkCopyWrapper : IBulkCopy
    {
        private readonly Microsoft.Data.SqlClient.SqlBulkCopy _inner;

        public SqlBulkCopyWrapper(Microsoft.Data.SqlClient.SqlBulkCopy inner)
        {
            _inner = inner;
        }

        public string DestinationTableName
        {
            get => _inner.DestinationTableName;
            set => _inner.DestinationTableName = value;
        }

        public IList<(string Source, string Destination)> ColumnMappings { get; }
            = new List<(string, string)>();

        public async Task WriteToServerAsync(DataTable table, CancellationToken cancellationToken)
        {
            foreach (var (src, dest) in ColumnMappings)
                _inner.ColumnMappings.Add(src, dest);

            await _inner.WriteToServerAsync(table, cancellationToken);
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
