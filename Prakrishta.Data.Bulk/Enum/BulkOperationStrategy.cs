namespace Prakrishta.Data.Bulk.Enum
{
    public enum BulkOperationStrategy
    {
        StoredProcedure,   // TVP + proc (current)
        StagingTable,       // SqlBulkCopy + set-based UPDATE/DELETE
        TruncateAndReload
    }
}
