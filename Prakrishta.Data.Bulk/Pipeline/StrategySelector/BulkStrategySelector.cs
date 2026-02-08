using Prakrishta.Data.Bulk.Core;
using Prakrishta.Data.Bulk.Enum;

namespace Prakrishta.Data.Bulk.Pipeline.StrategySelector
{
    public class BulkStrategySelector(BulkOptions options)
    {
        private readonly BulkOptions _options = options;

        public BulkStrategyKind Select(BulkContext context)
        {
            if (context.StrategyKind == BulkStrategyKind.PartitionSwitch)
                return BulkStrategyKind.PartitionSwitch;

            if (context.OperationKind == BulkOperationKind.ReplacePartition)
                return BulkStrategyKind.PartitionSwitch;

            if (context.OperationKind == BulkOperationKind.Insert 
                && context.ItemCount >= _options.StagingThreshold)
                return BulkStrategyKind.StagingTable;

            if (context.ItemCount >= _options.StagingThreshold)
                return BulkStrategyKind.StagingTable;

            return BulkStrategyKind.StoredProcedureTvp;
        }
    }

}
