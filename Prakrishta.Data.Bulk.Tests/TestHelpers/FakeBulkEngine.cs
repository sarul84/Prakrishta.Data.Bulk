using Prakrishta.Data.Bulk.Abstractions;

internal sealed class FakeBulkEngine : IBulkEngineInternal
{
    public IEnumerable<object>? LastItems { get; private set; }
    public string? LastTableName { get; private set; }
    public string? LastTvpName { get; private set; }
    public string? LastProcedureName { get; private set; }

    public Task<int> InsertAsync<T>(
        IEnumerable<T> items,
        string tableName,
        string? tableTypeName = null,
        string? procedureName = null,
        CancellationToken cancellationToken = default)
    {
        LastItems = items.Cast<object>().ToList();
        LastTableName = tableName;
        LastTvpName = tableTypeName;
        LastProcedureName = procedureName;
        return Task.FromResult(LastItems.Count());
    }

    public Task<int> UpdateAsync<T>(
        IEnumerable<T> items,
        string tableName,
        string? tableTypeName = null,
        string? procedureName = null,
        CancellationToken cancellationToken = default)
    {
        LastItems = items.Cast<object>().ToList();
        LastTableName = tableName;
        LastTvpName = tableTypeName;
        LastProcedureName = procedureName;
        return Task.FromResult(LastItems.Count());
    }

    public Task<int> DeleteAsync<T>(
        IEnumerable<T> items,
        string tableName,
        string? tableTypeName = null,
        string? procedureName = null,
        CancellationToken cancellationToken = default)
    {
        LastItems = items.Cast<object>().ToList();
        LastTableName = tableName;
        LastTvpName = tableTypeName;
        LastProcedureName = procedureName;
        return Task.FromResult(LastItems.Count());
    }
}