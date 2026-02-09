namespace Prakrishta.Data.Bulk.Core
{
    using Prakrishta.Data.Bulk.Enum;

    public sealed class PartitionSwitchOptions
    {
        public string? StagingTable { get; private set; }
        public int? PartitionNumber { get; private set; }

        public PartitionSwitchOptions UseStagingTable(string tableName)
        {
            StagingTable = tableName;
            return this;
        }

        public PartitionSwitchOptions ForPartition(int partitionNumber)
        {
            PartitionNumber = partitionNumber;
            return this;
        }

        /// <summary>
        /// Applies the configured options to the BulkContext by writing into Properties.
        /// </summary>
        internal void ApplyTo(BulkContext context)
        {
            if (context is null)
                throw new ArgumentNullException(nameof(context));

            if (StagingTable is null)
                throw new InvalidOperationException("Staging table must be specified before applying options.");

            if (PartitionNumber is null)
                throw new InvalidOperationException("Partition number must be specified before applying options.");

            // Set strategy explicitly
            context.StrategyKind = BulkStrategyKind.PartitionSwitch;

            // Store metadata in Properties
            context.Properties["StagingTable"] = StagingTable;
            context.Properties["PartitionNumber"] = PartitionNumber;
        }

    }
}
