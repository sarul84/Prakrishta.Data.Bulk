namespace Prakrishta.Data.Bulk.Enum
{
    public enum BulkOperationStrategy
    {
        StoredProcedure,
        StagingTable,
        TruncateAndReload,
        PartitionSwitch
    }
}
