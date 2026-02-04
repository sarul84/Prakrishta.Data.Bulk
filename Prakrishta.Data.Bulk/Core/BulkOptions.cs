namespace Prakrishta.Data.Bulk.Core
{
    using Prakrishta.Data.Bulk.Enum;
    public sealed class BulkOptions
    {
        public BulkStrategyKind PreferredStrategy { get; set; } = BulkStrategyKind.StagingTable;
        public int StagingThreshold { get; set; } = 5_000;
        public Func<string, string> StagingTableNameFactory { get; set; } = t => t + "_Staging";
    }

}
