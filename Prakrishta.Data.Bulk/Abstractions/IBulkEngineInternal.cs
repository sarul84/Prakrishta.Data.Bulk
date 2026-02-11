namespace Prakrishta.Data.Bulk.Abstractions;

internal interface IBulkEngineInternal
{
    Task<int> InsertAsync<T>(
            IEnumerable<T> items,
            string tableName,
            string? tableTypeName = null,
            string? procedureName = null,
            CancellationToken cancellationToken = default);
    Task<int> UpdateAsync<T>(IEnumerable<T> items,
            string tableName,
            string? tableTypeName = null,
            string? procedureName = null,
            CancellationToken cancellationToken = default);
    Task<int> DeleteAsync<T>(
            IEnumerable<T> items,
            string tableName,
            string? tableTypeName = null,
            string? procedureName = null,
            CancellationToken cancellationToken = default);
}