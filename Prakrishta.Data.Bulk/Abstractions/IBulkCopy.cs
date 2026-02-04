namespace Prakrishta.Data.Bulk.Abstractions
{
    using System.Data;

    public interface IBulkCopy : IAsyncDisposable
    {
        string DestinationTableName { get; set; }
        IList<(string Source, string Destination)> ColumnMappings { get; }

        Task WriteToServerAsync(DataTable table, CancellationToken cancellationToken);
    }
}
