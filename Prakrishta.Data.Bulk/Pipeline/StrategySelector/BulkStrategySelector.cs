using Prakrishta.Data.Bulk.Core;
using Prakrishta.Data.Bulk.Enum;

namespace Prakrishta.Data.Bulk.Pipeline.StrategySelector
{
    public sealed class BulkStrategySelector(BulkOptions options)
    {
        private readonly BulkOptions _options = options;

        public BulkStrategyKind Select(BulkContext context)
        {
            if (context.OperationKind == BulkOperationKind.Insert 
                && context.ItemCount >= _options.StagingThreshold)
                return BulkStrategyKind.StagingTable;

            if (context.ItemCount >= _options.StagingThreshold)
                return BulkStrategyKind.StagingTable;

            return BulkStrategyKind.StoredProcedureTvp;
        }
    }

}
