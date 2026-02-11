namespace Prakrishta.Data.Bulk.Abstractions
{
    internal interface ISchemaResolver
    {
        Task<string> ResolveSchemaAsync(string tableName, CancellationToken cancellationToken);
    }
}
