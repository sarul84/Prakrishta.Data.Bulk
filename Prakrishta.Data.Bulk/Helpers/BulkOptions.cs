using Prakrishta.Data.Bulk.Enum;

namespace Prakrishta.Data.Bulk.Helpers
{
    public sealed class BulkOptions
    {
        public BulkOperationStrategy PreferredStrategy { get; set; } = BulkOperationStrategy.StagingTable;

        public int StagingThreshold { get; set; } = 5_000;

        public Func<string, string> StagingTableNameFactory { get; set; }
            = target => target + "_Staging";
    }

}
